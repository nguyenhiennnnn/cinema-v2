import { apiPost } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { AiChatRequest, AiChatResponse } from "@/features/client/ai/types";
import { ENDPOINTS } from "@/api/endpoints";

export const aiApi = {
  chat: (body: AiChatRequest) =>
    apiPost<ApiResponse<AiChatResponse>>(ENDPOINTS.ai.chat, body),
};
