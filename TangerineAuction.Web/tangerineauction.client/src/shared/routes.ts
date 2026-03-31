import { createHistoryRouter, createRoute, RouteParams, RouteInstance, RouteQuery, createRouterControls, buildPath } from "atomic-router";
import { sample } from "effector";
import { createBrowserHistory } from "history";
import { appStarted } from "./init";
import { useRouter } from "atomic-router-react";

export const routes = {
    home: createRoute(),
    notfound: createRoute(),
    auction: createRoute<{ id: string }>(),
    lesson: createRoute(),
    developer: createRoute(),
};

export const mappedRoutes = [
    { path: "/", route: routes.home },
    { path: "/notfound", route: routes.notfound },
    { path: "/auction/:id", route: routes.auction },
    { path: "/developer", route: routes.developer },
];

export const controls = createRouterControls();

export const router = createHistoryRouter({
    routes: mappedRoutes,
    notFoundRoute: routes.notfound,
    controls,
});

sample({
    clock: appStarted,
    fn: () => createBrowserHistory(),
    target: router.setHistory,
});

export function usePath<T extends RouteParams>(route: RouteInstance<T>): (params: T, query?: RouteQuery) => string {
    const router = useRouter();
    const routeObj = router.routes.find((routeObj) => routeObj.route === route);

    if (!routeObj) {
        throw new Error("[@shared/routes] Route not found");
    }

    return (params, query = {}) =>
        buildPath({
            pathCreator: routeObj.path,
            params: params || {},
            query: query || {},
        });
}

export function useLink<T extends RouteParams>(route: RouteInstance<T>, params: T, query: RouteQuery = {}) {
    const builder = usePath(route);
    return builder(params, query);
}