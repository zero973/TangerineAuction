import { Anchor, Container, Stack, Button } from "@mantine/core";
import { generateTangerine } from "@pages/developer/model.ts";

export const DeveloperPage = () => {
    return (
        <Container>
            <Stack align="center" justify="center" gap="md">
                <Anchor href="https://localhost:10001/swagger" target="_blank" underline="hover">
                    Swagger
                </Anchor>
                <Anchor href="https://localhost:10001/healthchecks-ui" target="_blank" underline="hover">
                    Health checks
                </Anchor>
                <Anchor href="http://localhost:16686/search" target="_blank" underline="hover">
                    Jaeger
                </Anchor>
                <Anchor href="http://localhost:8080/" target="_blank" underline="hover">
                    Keycloak
                </Anchor>
                <Button onClick={() => generateTangerine()}>Сгенерировать мандаринку</Button>
            </Stack>
        </Container>
    );
};