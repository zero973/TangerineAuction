import { AuctionPage } from "@pages/auction/page.tsx";
import { auctionPageRoute } from "@pages/auction/model.ts";
import { PublicLayout } from "@shared/layouts/public-layout/ui.tsx";

export const AuctionRoute = {
    view: AuctionPage,
    route: auctionPageRoute,
    layout: PublicLayout
};