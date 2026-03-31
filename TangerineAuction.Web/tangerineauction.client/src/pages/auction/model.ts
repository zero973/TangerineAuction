import { chainRoute } from "atomic-router";
import { createEffect, createStore, sample } from "effector";
import { requestFx } from "@shared/api/request.ts";
import { showError } from "@shared/notifications";
import { routes } from "@shared/routes";
import { TangerineQuality } from "@pages/home/model.ts";

export const auctionPageRoute = routes.auction;

export type TangerineResponse = {
    id: string;
    name: string;
    quality: TangerineQuality;
    startPrice: number;
    filePath: string;
    createdOn: string;
};

export type BetResponse = {
    id: string;
    price: number;
    createdOn: string;
    createdBy: string;
};

export type AuctionFullResponse = {
    auctionId: string;
    name: string;
    tangerine: TangerineResponse;
    createdOn: string;
    bets: BetResponse[];
};

export type AddBetBody = {
    auctionId: string;
    price: number;
};

export const getAuction = createEffect<{ id: string }, AuctionFullResponse>((params) => {
    return requestFx({
        method: "GET",
        path: "Auctions/Get",
        params,
    });
});

export const canCreateBet = createEffect<{ auctionId: string }, boolean>(async (params) => {
    try {
        return await requestFx({
            method: "GET",
            path: "Bets/CanCreate",
            params,
        });
    } catch (error: any) {
        return false;
    }
});

export const addBet = createEffect<AddBetBody, BetResponse>((body) => {
    return requestFx({
        method: "POST",
        path: "Bets/Add",
        body,
    });
});

export const $auction = createStore<AuctionFullResponse | null>(null)
    .on(getAuction.doneData, (_, p) => p)
    .on(addBet.doneData, (auction, bet) => {
        if (!auction) return auction;

        return {
            ...auction,
            bets: [...auction.bets, bet],
        };
    });

export const $canCreateBetResult = createStore<boolean>(false)
    .on(canCreateBet.doneData, (_, p) => p)
    .on(addBet.doneData, () => {
        return false;
    });

export const loadAuctionPage = createEffect<{ id: string }, void>(async ({ id }) => {
    await Promise.all([
        getAuction({ id: id }),
        canCreateBet({ auctionId: id }),
    ]);
});

chainRoute({
    route: auctionPageRoute,
    beforeOpen: {
        effect: loadAuctionPage,
        mapParams: ({ params }) => ({ id: params.id }),
    },
});

sample({
    clock: loadAuctionPage.fail,
    fn: () => "Ошибка при загрузке аукциона",
    target: showError,
});

sample({
    clock: loadAuctionPage.fail,
    target: routes.home.open,
});