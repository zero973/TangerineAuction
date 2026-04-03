import { createEffect } from "effector";
import { requestFx } from "@shared/api/request";
import { BetResponse } from "@shared/api/contracts/bet";

export const getLastBetFx = createEffect<string, { id: string; bet: BetResponse }>(async (id) => {
    const bet = await requestFx({
        method: "GET",
        path: "Bets/GetLastBet",
        params: { id },
    });

    return { id, bet: bet as BetResponse };
});