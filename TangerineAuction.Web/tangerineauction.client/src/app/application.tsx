import "@mantine/core/styles.css";
import "@mantine/dates/styles.css";
import "react-toastify/dist/ReactToastify.css";
import "./styles.css";
import "./variables.css";
import "dayjs/locale/ru";

import { createTheme, MantineProvider } from "@mantine/core";
import { router } from "@shared/routes";
import { RouterProvider } from "atomic-router-react";
import { Pages } from "@pages/index";
import { appStarted } from "@shared/init";
import { ToastContainer } from "react-toastify";
import { DatesProvider } from "@mantine/dates";

appStarted();

const theme = createTheme({
    fontFamily: "Open Sans, sans-serif",
    primaryColor: "dark",
});

export default function Application() {
    return (
        <RouterProvider router={router}>
            <MantineProvider theme={theme}>
                <DatesProvider
                    settings={{
                        locale: "en",
                        firstDayOfWeek: 0,
                        weekendDays: [0, 6],
                    }}
                >
                    <ToastContainer />
                    <Pages />
                </DatesProvider>
            </MantineProvider>
        </RouterProvider>
    );
}