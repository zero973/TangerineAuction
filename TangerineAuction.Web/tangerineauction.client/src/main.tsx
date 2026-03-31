import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import Application from "./app/application.tsx";

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Application />
  </StrictMode>,
)
