export interface ChatMessageDto {
  role: string;
  content: string;
}

export interface AiChatRequest {
  message: string;
  history?: ChatMessageDto[] | null;
}

export interface AiChatResponse {
  reply: string;
  timestamp: string;
}
