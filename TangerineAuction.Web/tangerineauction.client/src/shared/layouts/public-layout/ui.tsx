import { AppShell, Image, LoadingOverlay, Progress, Stack } from "@mantine/core";
import { PropsWithChildren } from "react";
import { MainHeader } from "../header/ui";
import { useUnit } from "effector-react";
import { $loadingApp } from "./model";
import ImageLogo from "../header/assets/image.gif";

export const PublicLayout = ({ children }: PropsWithChildren) => {
    const loadingApp = useUnit($loadingApp);

    if (loadingApp < 100) {
        return (
            <LoadingOverlay
                visible
                transitionProps={{
                    duration: 200,
                }}
                loaderProps={{
                    children: (
                        <Stack align="center">
                            <Image src={ImageLogo} w={"100px"} />

                            <Progress.Root size="xl" w="300px" transitionDuration={200}>
                                <Progress.Section color="red" value={loadingApp}>
                                    <Progress.Label>Loading</Progress.Label>
                                </Progress.Section>
                            </Progress.Root>
                        </Stack>
                    ),
                }}
            />
        );
    }

    return (
        <AppShell header={{ height: 90 }} pos="relative" padding="md">
            <AppShell.Header>
                <AppShell.Header>
                    <MainHeader />
                </AppShell.Header>
            </AppShell.Header>

            <AppShell.Main>{children}</AppShell.Main>
        </AppShell>
    );
};