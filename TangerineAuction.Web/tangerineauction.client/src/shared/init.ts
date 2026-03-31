import { createEffect, createEvent, sample } from "effector";
import { initAuthFx } from "@shared/api/auth/model";

export const appStarted = createEvent();

const triggerApp = createEffect(() => {
    console.log("init");
});

sample({
    clock: appStarted,
    target: triggerApp,
});

sample({
    clock: appStarted,
    target: initAuthFx,
});