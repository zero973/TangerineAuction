import { createEffect } from "effector";
import { toast, ToastOptions } from "react-toastify";
import React from "react";

type ToastPayload =
    | string
    | {
    title?: string;
    message: string;
    autoCloseMs?: number;
    options?: Omit<ToastOptions, "autoClose" | "render">;
};

const computeAutoClose = (
    message: string,
    { base = 2000, perChar = 40, min = 2000, max = 8000 } = {}
): number => {
    const ms = base + message.length * perChar;
    return Math.max(min, Math.min(ms, max));
};

const renderToastContent = (payload: ToastPayload): React.ReactNode => {
    const title = typeof payload === "string" ? undefined : payload.title;
    const message = typeof payload === "string" ? payload : payload.message;

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: 6 }}>
            {title && (
                <div style={{ fontWeight: 600, fontSize: 14, lineHeight: 1 }}>
                    {title}
                </div>
            )}
            <div style={{ whiteSpace: "pre-wrap", fontSize: 13 }}>{message}</div>
        </div>
    );
};

const makeToastEffect = (type: "error" | "success" | "info") =>
    createEffect((payload: ToastPayload) => {
        const message = typeof payload === "string" ? payload : payload.message;
        const userAutoClose =
            typeof payload === "object" ? payload.autoCloseMs : undefined;
        const autoClose = userAutoClose ?? computeAutoClose(message);

        const options: ToastOptions = {
            autoClose,
            closeOnClick: true,
            pauseOnHover: true,
            ...(typeof payload === "object" ? payload.options ?? {} : {}),
        };

        return toast[type](renderToastContent(payload), options);
    });

export const showError = makeToastEffect("error");
export const showSuccess = makeToastEffect("success");
export const showInfo = makeToastEffect("info");