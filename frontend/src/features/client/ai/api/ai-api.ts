import { apiPost } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { AiChatRequest, AiChatResponse } from "../types";

export const aiApi = {
  chat: (body: AiChatRequest) =>
    apiPost<ApiResponse<AiChatResponse>>("/ai/chat", body),
};
