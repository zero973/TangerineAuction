import axios, { ResponseType } from "axios";
import { createEffect, createEvent, createStore, sample } from "effector";
import { Mutex } from "async-mutex";
import { doRefreshFx, tokenExpired } from "@shared/api/auth/model.ts";

const MAX_ERRORS = 3;
const CHECK_INTERVAL = 30000;

interface ServerStatus {
    isAvailable: boolean;
    errorCount: number;
    lastCheck: number;
}

export const $serverStatus = createStore<ServerStatus>({
    isAvailable: true,
    errorCount: 0,
    lastCheck: Date.now(),
});

export const serverError = createEvent<void>();
export const serverRecovered = createEvent<void>();
export const resetErrorCount = createEvent<void>();

export const checkServerStatusFx = createEffect(async (): Promise<boolean> => {
    try {
        const response = await requestFx({
            path: "health",
            method: "GET",
            returnFullResponse: true,
        });

        const statusCode = response?.status;
        const data = response?.data;

        if (statusCode === 200) {
            const overall = (data?.status ?? "").toString().toLowerCase();
            return overall === "healthy" || overall === "degraded";
        }

        if (statusCode === 503) return false;

        return false;
    } catch (err: any) {
        const status = err?.response?.status;
        if (typeof status === "number") {
            if (status === 200) return true;
            if (status === 503) return false;
            if (status < 500) return true;
        }
        return false;
    }
});

$serverStatus
    .on(serverError, (state) => ({
        ...state,
        errorCount: state.errorCount + 1,
        isAvailable: state.errorCount + 1 < MAX_ERRORS,
        lastCheck: Date.now(),
    }))
    .on(serverRecovered, (state) => ({
        ...state,
        errorCount: 0,
        isAvailable: true,
        lastCheck: Date.now(),
    }))
    .on(resetErrorCount, (state) => ({
        ...state,
        errorCount: 0,
    }));

let checkInterval: any | null = null;

sample({
    source: $serverStatus,
    filter: (status) => !status.isAvailable,
    fn: () => {},
    target: createEffect(() => {
        if (!checkInterval) {
            checkInterval = setInterval(async () => {
                const isAvailable = await checkServerStatusFx();
                if (isAvailable) {
                    serverRecovered();
                    if (checkInterval) {
                        clearInterval(checkInterval);
                        checkInterval = null;
                    }
                    window.location.reload();
                }
            }, CHECK_INTERVAL);
        }
    }),
});

const mutex = new Mutex();

export const api = axios.create({
    baseURL: "/api",
    timeout: 60000,
    withCredentials: true,
});

interface Request {
    path: string;
    method: "GET" | "POST" | "PUT" | "DELETE" | "PATCH";
    body?: unknown;
    params?: unknown;
    headers?: unknown;
    responseType?: ResponseType;
    withReturnValue?: unknown;
    returnFullResponse?: boolean;
}

export const requestFx = createEffect<Request, any>(async (request) => {
    const language = localStorage.getItem("language") || "en";

    const getAccessToken = (): string | null => {
        try {
            const raw = localStorage.getItem("token");
            if (!raw) return null;

            const parsed = JSON.parse(raw) as { accessToken?: string | null };
            return parsed.accessToken ?? null;
        } catch {
            return null;
        }
    };

    const createHeaders = (additionalHeaders: Record<string, string> = {}) => {
        const accessToken = getAccessToken();

        return {
            "Accept-Language": language,
            ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
            ...additionalHeaders,
        };
    };

    const executeRequest = async () => {
        const response = await api({
            method: request.method,
            url: request.path,
            data: request.body,
            headers: createHeaders(request.headers as Record<string, string>),
            params: request.params,
            responseType: request.responseType,
            withCredentials: true,
        });

        if (request?.withReturnValue) {
            return { ...response.data, ...request?.withReturnValue };
        } else if (request?.returnFullResponse) {
            return response;
        }

        return response.data;
    };

    await mutex.waitForUnlock();

    try {
        const result = await executeRequest();
        serverRecovered();
        return result;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            const status = error.response?.status;

            if (status === 502) {
                console.log(error);
                serverError();
                throw error;
            }

            if (status === 401) {
                const newToken = await doRefreshFx();

                if (newToken?.accessToken) {
                    localStorage.setItem("token", JSON.stringify(newToken));

                    const retryResponse = await api({
                        method: request.method,
                        url: request.path,
                        data: request.body,
                        headers: createHeaders(request.headers as Record<string, string>),
                        params: request.params,
                        responseType: request.responseType,
                        withCredentials: true,
                    });

                    if (request?.withReturnValue) {
                        return { ...retryResponse.data, ...request?.withReturnValue };
                    } else if (request?.returnFullResponse) {
                        return retryResponse;
                    }

                    return retryResponse.data;
                }

                tokenExpired();
                throw new Error("Token refresh failed");
            }
        }

        throw error;
    }
});