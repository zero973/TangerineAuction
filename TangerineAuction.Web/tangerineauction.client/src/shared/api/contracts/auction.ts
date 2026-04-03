import { TangerineQuality, TangerineResponse} from "./tangerine";
import { BetResponse } from "@shared/api/contracts/bet.ts";

export type AuctionSearchParams = {
    skip?: number;
    take?: number;
    auctionName?: string | null;
    tangerineName?: string | null;
    tangerineQuality?: number | null;
    showFinishedAuctions: boolean;
    isCurrentUserWinner: boolean;
};

export type AuctionResponse = {
    auctionId: string;
    auctionName: string;
    isActual: boolean;
    auctionCreatedOn: string;
    tangerineName: string;
    tangerineQuality: TangerineQuality;
    imageUrl: string;
    lastBet: number;
    lastUserBetId: string;
};

export type AuctionFullResponse = {
    auctionId: string;
    name: string;
    isActual: boolean;
    tangerine: TangerineResponse;
    createdOn: string;
    bets: BetResponse[];
};