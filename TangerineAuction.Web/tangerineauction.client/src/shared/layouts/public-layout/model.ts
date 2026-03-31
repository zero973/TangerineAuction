import { getAuctions } from "@pages/home/model.ts";
import { getUserFx } from "@shared/api/auth/model";
import { appStarted } from "@shared/init";
import { createEvent, createStore, sample } from "effector";
import { debounce } from "patronum";

const handleLoading = createEvent<number>();

sample({
    clock: debounce(getAuctions.done, 500),
    fn() {
        return 50;
    },
    target: handleLoading,
});

sample({
    clock: debounce(getUserFx.done, 500),
    fn() {
        return 50;
    },
    target: handleLoading,
});

export const $loadingApp = createStore(0).on(handleLoading, (s, p) => (s + p > 100 ? 100 : s + p));

sample({
    clock: debounce(appStarted, 2500),
    fn: () => 100,
    target: handleLoading,
});