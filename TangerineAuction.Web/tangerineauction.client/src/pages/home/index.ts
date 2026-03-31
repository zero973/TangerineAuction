import { HomePage } from "./page";
import { homePageRoute, notfoundPageRoute } from "./model";
import { PublicLayout } from "@shared/layouts/public-layout/ui";

export const HomeRoute = {
    view: HomePage,
    route: homePageRoute,
    layout: PublicLayout,
};

export const NotfoundRoute = {
    view: HomePage,
    route: notfoundPageRoute,
    layout: PublicLayout,
};