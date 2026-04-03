import { requestFx } from "@shared/api/request";
import { routes } from "@shared/routes";
import { chainRoute } from "atomic-router";
import { createEffect, createStore, sample, createEvent } from "effector";
import { debounce } from "patronum";
import { getLastBetFx } from "@shared/api/bets/model.ts";
import { hubMessageReceived } from "@shared/api/signalr/model.ts";
import { HubMessageType } from "@shared/api/contracts/hub.ts";
import { showSuccess } from "@shared/notifications.tsx";
import { AuctionResponse, AuctionSearchParams } from "@shared/api/contracts/auction.ts";

type Pagination = { skip: number; take: number };

export const homePageRoute = routes.home;
export const notfoundPageRoute = routes.notfound;

export const getAuctions = createEffect<AuctionSearchParams, AuctionResponse[]>((params) => {
    return requestFx({
        method: "GET",
        path: "Auctions/GetAll",
        params,
    });
});

export const searchAuctions = createEvent<AuctionSearchParams>();
export const goNextPage = createEvent();
export const goPrevPage = createEvent();

export const $auctions = createStore<AuctionResponse[]>([])
    .on(getAuctions.doneData, (_, p) => p)
    .on(getLastBetFx.doneData, (auctions, { id, bet }) =>
        auctions.map((auction) =>
            auction.auctionId === id
                ? {
                    ...auction,
                    lastBet: bet.price,
                    lastUserBetId: bet.createdBy,
                }
                : auction
        )
    )
    .on(hubMessageReceived, (auctions, msg) => {
        if (msg.type !== HubMessageType.AuctionFinished) {
            return auctions;
        }

        return auctions.map((auction) =>
            auction.auctionId === msg.entityId
                ? { ...auction, isActual: false }
                : auction
        );
    });

export const $pagination = createStore<{ skip: number; take: number }>({ skip: 0, take: 10 })
    .on(searchAuctions, (_, payload) => ({ skip: payload.skip ?? 0, take: payload.take ?? 10 }))
    .on(goNextPage, (s) => ({ ...s, skip: s.skip + s.take }))
    .on(goPrevPage, (s) => ({ ...s, skip: Math.max(0, s.skip - s.take) }));

sample({
    clock: debounce(searchAuctions, 300),
    source: $pagination,
    fn: (page: Pagination, payload?: AuctionSearchParams): AuctionSearchParams => {
        return {
            skip: payload?.skip ?? page.skip,
            take: payload?.take ?? page.take,
            auctionName: payload?.auctionName ?? null,
            tangerineName: payload?.tangerineName ?? null,
            tangerineQuality: payload?.tangerineQuality ? Number(payload.tangerineQuality)-1 : null,
            showFinishedAuctions: payload?.showFinishedAuctions ?? false,
            isCurrentUserWinner: payload?.isCurrentUserWinner ?? false,
        };
    },
    target: getAuctions,
});

sample({
    clock: hubMessageReceived,
    source: $auctions,
    filter: (auctions, msg) =>
        msg.type === HubMessageType.NewBetAdded &&
        auctions.some((auction) => auction.auctionId === msg.entityId),
    fn: (_, msg) => msg.entityId,
    target: getLastBetFx,
});

sample({
    clock: hubMessageReceived,
    filter: (msg) => msg.type === HubMessageType.AuctionAdded,
    fn: () => "Создан новый аукцион",
    target: showSuccess,
});

chainRoute({
    route: homePageRoute,
    beforeOpen: {
        effect: getAuctions,
        mapParams: () => ({ skip: 0, take: 10 } as AuctionSearchParams),
    },
});