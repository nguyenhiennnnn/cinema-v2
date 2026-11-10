export interface ChatMessageDto {
  role: string;
  content: string;
}

export interface AiChatRequest {
  message: string;
  history?: ChatMessageDto[] | null;
}

export type AiCardType = "showtime" | "booking";

export interface AiCard {
  type: AiCardType;
  title: string;
  subtitle: string | null;
  detail: string | null;
  imageUrl: string | null;
  link: string;
}

export interface AiChatResponse {
  reply: string;
  timestamp: string;
  cards: AiCard[];
}