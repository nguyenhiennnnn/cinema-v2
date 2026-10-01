// ── Room ──────────────────────────────────────────────────────────────────────

export interface AdminRoomDto {
  roomId: number;
  roomName: string;
  screenTypeId?: string | null;
  screenTypeName?: string | null;
  totalSeats: number;
  isActive: boolean;
  rows: number;
  cols: number;
}

export interface CreateRoomRequest {
  roomName: string;
  screenTypeId?: number | null;
  rows: number;
  cols: number;
}

export interface UpdateRoomRequest {
  roomName: string;
  screenTypeId?: number | null;
  isActive: boolean;
}

export interface DeleteRoomResponse {
  roomId: number;
  deleted: boolean;
  softDeleted: boolean;
}

export interface RoomListParams {
  page?: number;
  pageSize?: number;
}

// ── Seat layout ───────────────────────────────────────────────────────────────

export interface SeatDto {
  seatId: number;
  seatRow: string;
  seatNumber: number;
  seatTypeId?: number | null;
  seatTypeName?: string | null;
  isActive: boolean;
  isAisle: boolean;
  coupleId?: string | null;
}

export interface SeatLayoutItem {
  seatRow: string;
  seatNumber: number;
  seatTypeId?: number | null;
  isActive: boolean;
  isAisle: boolean;
  coupleId?: string | null;
}

export interface UpdateSeatLayoutRequest {
  rows: number;
  cols: number;
  seats: SeatLayoutItem[];
}

export interface UpdateSeatLayoutResponse {
  created: number;
  updated: number;
  deactivated: number;
  removed: number;
  totalSeats: number;
}
