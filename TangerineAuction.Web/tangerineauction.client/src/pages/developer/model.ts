import { createEffect } from "effector";
import { routes } from "@shared/routes";
import { requestFx } from "@shared/api/request.ts";

export const developerPageRoute = routes.developer;

export const generateTangerine = createEffect<void, any>(() => {
    return requestFx({
        method: "POST",
        path: "Tangerines/Generate",
    });
});