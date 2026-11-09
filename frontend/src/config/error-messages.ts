export interface StatusErrorRule {
  message: string;
  preferServerMessage?: boolean;
}

export const STATUS_ERROR_RULES: Readonly<Record<number, StatusErrorRule>> = {
  400: { message: "Dữ liệu không hợp lệ.", preferServerMessage: true },
  401: { message: "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại." },
  403: { message: "Bạn không có quyền thực hiện thao tác này." },
  404: { message: "Không tìm thấy dữ liệu yêu cầu." },
  409: {
    message: "Dữ liệu bị xung đột. Vui lòng thử lại.",
    preferServerMessage: true,
  },
  422: { message: "Dữ liệu không hợp lệ.", preferServerMessage: true },
  429: { message: "Bạn thao tác quá nhanh. Vui lòng thử lại sau." },
};

export const SERVER_ERROR_RULE: StatusErrorRule = {
  message: "Máy chủ đang gặp sự cố. Vui lòng thử lại sau.",
};

export const ERROR_MESSAGES = {
  offline: "Không có kết nối mạng. Vui lòng kiểm tra lại.",
  network: "Không kết nối được máy chủ. Kiểm tra mạng rồi thử lại.",
  timeout: "Máy chủ phản hồi quá lâu. Vui lòng thử lại.",
  fallback: "Đã có lỗi xảy ra. Vui lòng thử lại.",
  unknown: "Đã có lỗi xảy ra không xác định",
  toastTitle: "Đã có lỗi xảy ra",
} as const;

export const SILENT_TOAST_STATUSES: readonly number[] = [401];

export const ERROR_TOAST_DEDUPE_MS = 3000;
