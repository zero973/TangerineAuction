import { requestFx } from "@shared/api/request";
import { routes } from "@shared/routes";
import { chainRoute } from "atomic-router";
import { createEffect, createStore, sample, createEvent } from "effector";
import { debounce } from "patronum";

export type AuctionSearchParams = {
    skip?: number;
    take?: number;
    name?: string | null;
};

export enum TangerineQuality {
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Legendary = 3,
}

export type AuctionResponse = {
    auctionId: string;
    auctionName: string;
    auctionCreatedOn: string;
    tangerineName: string;
    tangerineQuality: TangerineQuality;
    filePath: string;
    lastBet: number;
    lastUserBetId: string;
};

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

export const $auctions = createStore<AuctionResponse[]>([]).on(getAuctions.doneData, (_, p) => p);

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
            name: payload?.name ?? null,
        };
    },
    target: getAuctions,
});

chainRoute({
    route: homePageRoute,
    beforeOpen: {
        effect: getAuctions,
        mapParams: () => ({ skip: 0, take: 10 } as AuctionSearchParams),
    },
});