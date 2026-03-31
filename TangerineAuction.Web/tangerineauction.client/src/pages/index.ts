import { createRoutesView } from "atomic-router-react";
import { DeveloperRoute } from "@pages/developer";
import { HomeRoute, NotfoundRoute } from "@pages/home";
import { AuctionRoute } from "@pages/auction";

export const Pages = createRoutesView({
    routes:
        [
            HomeRoute,
            DeveloperRoute,
            AuctionRoute,
            NotfoundRoute,
        ]
});