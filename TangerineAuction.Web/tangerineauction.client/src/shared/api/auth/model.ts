import { createEffect, createEvent, createStore, sample } from "effector";
import Keycloak from "keycloak-js";

export const tokenExpired = createEvent();

const keycloak = new Keycloak({
    url: import.meta.env.VITE_KEYCLOAK_URL ?? "http://localhost:8080/", 
    realm: import.meta.env.VITE_KEYCLOAK_REALM ?? "tangerine_auction",
    clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID ?? "public-client",
});

type UserInfo = {
    id: string;
    username?: string;
    email?: string;
    firstName?: string;
    lastName?: string;
    isAdmin: boolean;
};

type KeycloakProfileLike = {
    id?: string;
    username?: string;
    email?: string;
    firstName?: string;
    lastName?: string;
};

type StoredToken = {
    accessToken: string;
    refreshToken: string | null;
};

function mapUser(profile: KeycloakProfileLike): UserInfo {
    const token = keycloak.tokenParsed as any;
    const roles: string[] = token?.realm_access?.roles ?? [];
    
    return {
        id: keycloak.subject ?? profile.id ?? "",
        username: profile.username ?? token?.preferred_username,
        email: profile.email ?? token?.email,
        firstName: profile.firstName ?? token?.given_name,
        lastName: profile.lastName ?? token?.family_name,
        isAdmin: roles.includes("Admin"),
    };
}

function persistToken(): StoredToken | null {
    const accessToken = keycloak.token ?? null;
    const refreshToken = keycloak.refreshToken ?? null;

    if (!accessToken) {
        return null;
    }

    const newToken: StoredToken = {
        accessToken,
        refreshToken,
    };

    localStorage.setItem("token", JSON.stringify(newToken));
    return newToken;
}

keycloak.onTokenExpired = () => {
    tokenExpired();
};

keycloak.onAuthSuccess = () => {
    persistToken();
};

export const doLoginFx = createEffect<{ redirectUri?: string } | void, void>(async (payload) => {
    try {
        const redirectUri = (payload as any)?.redirectUri ?? window.location.href;
        await keycloak.login({ redirectUri });
        persistToken();
    } catch (err) {
        console.error("Login failed", err);
    }
});

export const doLogoutFx = createEffect<void, void>(async () => {
    try {
        localStorage.removeItem("token");
        tokenExpired();

        await keycloak.logout({ redirectUri: window.location.origin });
    } catch (err) {
        console.warn("Logout failed", err);
        localStorage.removeItem("token");
        tokenExpired();
        window.location.href = "/";
    }
});

export const doRefreshFx = createEffect<void, StoredToken | null>(async () => {
    try {
        await keycloak.updateToken(30);
        return persistToken();
    } catch (err) {
        console.warn("Refresh failed", err);
        return null;
    }
});

export const initAuthFx = createEffect<void, boolean>(async () => {
    try {
        const authenticated = await keycloak.init({
            onLoad: "check-sso",
            silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
            silentCheckSsoFallback: false,
            checkLoginIframe: false,
        });

        if (authenticated) {
            persistToken();
        }
        
        return Boolean(authenticated);
    } catch (err) {
        console.error("Keycloak init failed", err);
        return false;
    }
});

export const getUserFx = createEffect<void, UserInfo>(async () => {
        const profile = (await keycloak.loadUserProfile()) as KeycloakProfileLike;
    return mapUser(profile);
});

export const $isAuth = createStore<boolean>(false)
    .on(initAuthFx.doneData, (_, v) => Boolean(v))
    .on(doRefreshFx.doneData, (_, v) => Boolean(v))
    .on(getUserFx.doneData, (_, u) => Boolean(u?.id))
    .reset(tokenExpired);

export const $user = createStore<UserInfo | null>(null)
    .on(getUserFx.doneData, (_, u) => u)
    .reset(tokenExpired);

sample({
    clock: initAuthFx.doneData,
    filter: (res) => Boolean(res),
    target: getUserFx,
});

sample({
    clock: doRefreshFx.doneData,
    filter: (res) => Boolean(res),
    target: getUserFx,
});

sample({
    clock: doRefreshFx.fail,
    target: tokenExpired,
});