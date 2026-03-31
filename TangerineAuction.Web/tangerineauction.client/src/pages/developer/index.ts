import { DeveloperPage } from "./page";
import { developerPageRoute } from "./model";
import { chainAuthorized } from "@shared/api/auth/middleware";
import { PublicLayout } from "@shared/layouts/public-layout/ui.tsx";

export const DeveloperRoute = {
    view: DeveloperPage,
    route: chainAuthorized(developerPageRoute),
    layout: PublicLayout
};