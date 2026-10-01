import { useMutation } from "@tanstack/react-query";
import { aiApi } from "../api/ai-api";
import type { AiChatRequest } from "../types";

export function useAiChat() {
  return useMutation({
    mutationFn: async (body: AiChatRequest) => {
      const res = await aiApi.chat(body);
      return res.data;
    },
  });
}
