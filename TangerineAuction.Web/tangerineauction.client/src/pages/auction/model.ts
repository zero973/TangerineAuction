import { chainRoute } from "atomic-router";
import { createEffect, createStore, sample } from "effector";
import { requestFx } from "@shared/api/request.ts";
import { showError } from "@shared/notifications";
import { routes } from "@shared/routes";
import { getLastBetFx } from "@shared/api/bets/model.ts";
import { hubMessageReceived } from "@shared/api/signalr/model.ts";
import { HubMessageType } from "@shared/api/contracts/hub.ts";
import { AuctionFullResponse } from "@shared/api/contracts/auction.ts";
import { AddBetBody, BetResponse } from "@shared/api/contracts/bet.ts";

export const auctionPageRoute = routes.auction;

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

export const buyTangerine = createEffect<{ id: string }, null>((body) => {
    return requestFx({
        method: "POST",
        path: "Bets/BuyTangerine",
        body,
    });
});

export const $auction = createStore<AuctionFullResponse | null>(null)
    .on(getAuction.doneData, (_, p) => p)
    .on(getLastBetFx.doneData, (auction, { id, bet }) => {
        if (!auction || auction.auctionId !== id) {
            return auction;
        }

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
    clock: hubMessageReceived,
    source: $auction,
    filter: (auction, msg) =>
        Boolean(auction) &&
        auction!.auctionId === msg.entityId &&
        msg.type === HubMessageType.AuctionFinished,
    fn: (_, msg) => ({ id: msg.entityId }),
    target: getAuction,
});

sample({
    clock: hubMessageReceived,
    source: $auction,
    filter: (auction, msg) =>
        Boolean(auction) &&
        auction!.auctionId === msg.entityId &&
        msg.type === HubMessageType.NewBetAdded,
    fn: (_, msg) => ({ id: msg.entityId }),
    target: getAuction,
});

sample({
    clock: getAuction.done,
    fn: ({ params }) => ({ auctionId: params.id }),
    target: canCreateBet,
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