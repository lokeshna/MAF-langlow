import React from "react";
import { createRoot } from "react-dom/client";
import { CopilotKit } from "@copilotkit/react-core";
import { CopilotPopup } from "@copilotkit/react-ui";

const rootElement = document.getElementById("copilotkit-root");

if (rootElement) {
  const root = createRoot(rootElement);

  root.render(
    React.createElement(
      CopilotKit,
      {
        runtimeUrl: "/api/copilotkit"
      },
      React.createElement(CopilotPopup, {
        labels: {
          title: "Langflow Assistant",
          initial: "Hi! I am connected to Langflow through your ASP.NET backend.",
        },
        instructions: "Answer clearly and concisely.",
        defaultOpen: false,
      })
    )
  );
}
