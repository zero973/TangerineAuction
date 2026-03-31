import { chainRoute, RouteInstance, RouteParams, RouteParamsAndQuery } from "atomic-router";
import { createEvent, sample } from "effector";
import { $isAuth, doLoginFx } from "./model";

export function chainAuthorized<Params extends RouteParams>(route: RouteInstance<Params>) {
    const checkSessionStarted = createEvent<RouteParamsAndQuery<Params>>();

    const alreadyAuthorized = sample({
        clock: checkSessionStarted,
        filter: $isAuth,
    });

    const forbidden = sample({
        clock: checkSessionStarted,
        filter: $isAuth.map((isAuth) => !isAuth),
    });

    sample({
        clock: forbidden,
        fn: () => ({ redirectUri: window.location.href }),
        target: doLoginFx,
    });

    return chainRoute({
        route,
        beforeOpen: checkSessionStarted,
        openOn: [alreadyAuthorized],
        cancelOn: [forbidden],
    });
}