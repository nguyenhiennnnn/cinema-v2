import { QRCodeSVG } from "qrcode.react";
import { cn } from "@/lib/utils";

interface PerfoTicketProps {
  /** Tên phim */
  movieTitle: string;
  /** Suất chiếu: e.g. "19:45 · 24/10/2026" */
  showtime: string;
  /** Phòng chiếu: e.g. "3D Experience Hall 03" */
  room: string;
  /** Ghế: e.g. "A4" hoặc "H12, H13" */
  seat: string;
  /** Tổng tiền: e.g. "95.000 VNĐ" */
  totalAmount: string;
  /** Mã vé: e.g. "BK20261023XGJZ-A4" */
  ticketCode: string;
  /** QR value: data URL image hoặc raw text */
  qrValue: string;
  /** Đã được soát vé chưa */
  isCheckedIn?: boolean;
  /** Extra class cho wrapper */
  className?: string;
}

/**
 * Perfo Ticket — React component chuẩn nhận diện Perfo từ perfo-ticket-template.svg.
 *
 * Layout 920×380:
 * - Nửa trái (thân vé cream):
 *   + Hàng trên: Brand Logo & Wordmark Perfo.
 *   + PHIM: Tên phim to đậm, rõ ràng.
 *   + Cột trái: SUẤT CHIẾU & PHÒNG xếp dọc riêng biệt (chứa thoải mái tên phòng dài).
 *   + Cột phải: Khối GHẾ nổi bật riêng biệt trong khung vé phụ màu ấm Perfo.
 *   + Dưới: Đường phân cách đục lỗ & TỔNG TIỀN đỏ Perfo.
 * - Nửa phải (cuống đỏ Perfo):
 *   + QR code quét vé chuẩn tâm, mã vé định danh & hướng dẫn.
 *   + Overlay "ĐÃ SOÁT VÉ" khi isCheckedIn=true.
 */
export const PerfoTicket = ({
  movieTitle,
  showtime,
  room,
  seat,
  totalAmount,
  ticketCode,
  qrValue,
  isCheckedIn = false,
  className,
}: PerfoTicketProps) => {
  // Tránh tràn tên phim quá dài
  const title = movieTitle.length > 25 ? movieTitle.slice(0, 24) + "…" : movieTitle;
  const isImageQr = qrValue.startsWith("data:image") || qrValue.startsWith("http");

  return (
    <div className={cn("w-full max-w-3xl select-none", className)}>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 920 380"
        width="100%"
        role="img"
        aria-label={`Vé điện tử Perfo — ${movieTitle} — Ghế ${seat}`}
        className="block drop-shadow-sm"
      >
        {/* === Defs === */}
        <defs>
          <clipPath id="stub-clip">
            <rect x="640" width="280" height="380" />
          </clipPath>
          <clipPath id="wordmark-clip">
            <rect x="-2" y="0" width="190" height="48" />
          </clipPath>
        </defs>

        {/* === Thân vé tổng thể === */}
        {/* Cream background (#FFFDFB) */}
        <path
          d="M18 10H618A22 22 0 0 0 662 10H902A8 8 0 0 1 910 18V362A8 8 0 0 1 902 370H662A22 22 0 0 0 618 370H18A8 8 0 0 1 10 362V18A8 8 0 0 1 18 10Z"
          fill="#FFFDFB"
          stroke="#170C09"
          strokeOpacity="0.18"
          strokeWidth="2"
        />
        {/* Red stub overlay (#970013) */}
        <path
          d="M18 10H618A22 22 0 0 0 662 10H902A8 8 0 0 1 910 18V362A8 8 0 0 1 902 370H662A22 22 0 0 0 618 370H18A8 8 0 0 1 10 362V18A8 8 0 0 1 18 10Z"
          fill="#970013"
          clipPath="url(#stub-clip)"
        />

        {/* === Perforation dots dọc ranh giới cuống vé === */}
        {[40, 64, 88, 112, 136, 160, 184, 208, 232, 256, 280, 304, 328].map((cy) => (
          <circle key={cy} cx="640" cy={cy} r="3.5" fill="#B97F12" />
        ))}

        {/* === Logo BrandMark Perfo === */}
        <g transform="translate(46.8 26) scale(0.75)">
          <path
            d="M6 14H35A5 5 0 0 0 45 14H58A2 2 0 0 1 60 16V48A2 2 0 0 1 58 50H45A5 5 0 0 0 35 50H6A2 2 0 0 1 4 48V16A2 2 0 0 1 6 14Z"
            fill="#970013"
          />
          <g fill="#F9F5F0">
            {[22, 27, 32, 37, 42].map((cy) => (
              <circle key={cy} cx="40" cy={cy} r="1.5" />
            ))}
            <rect x="47.5" y="24" width="2" height="16" />
            <rect x="51.5" y="26" width="1.2" height="12" />
            <rect x="54.5" y="24" width="2" height="16" />
          </g>
          <path
            d="M15 43V23.5H21.5L24 26V30.5L21.5 33H15"
            fill="none"
            stroke="#F9F5F0"
            strokeWidth="5"
            strokeMiterlimit="10"
          />
        </g>

        {/* === "PERFO" wordmark === */}
        <g transform="translate(100 36) scale(0.55)">
          <g clipPath="url(#wordmark-clip)" fill="none" stroke="#170C09" strokeWidth="9" strokeLinejoin="miter" strokeMiterlimit="10">
            <path transform="translate(0 0)" d="M4.5 48V4.5H23L27.5 9V24L23 28.5H4.5" />
            <path transform="translate(38 0)" d="M27.5 4.5H4.5V43.5H27.5M4.5 24H23" />
            <path transform="translate(76 0)" d="M4.5 48V4.5H23L27.5 9V24L23 28.5H4.5M14 28.5L30.3 52" />
            <path transform="translate(114 0)" d="M27.5 4.5H4.5V48M4.5 24H23" />
            <path transform="translate(152 0)" d="M9 4.5H23L27.5 9V39L23 43.5H9L4.5 39V9Z" />
          </g>
          <circle cx="168" cy="24" r="4.2" fill="#970013" />
        </g>

        {/* === THÔNG TIN PHIM === */}
        <text
          x="50" y="105"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="11" fontWeight="600" fill="#60524B"
          letterSpacing="2.5"
        >
          PHIM
        </text>
        <text
          x="50" y="140"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize="32" fontWeight="700" fill="#170C09"
          letterSpacing="0.3"
        >
          {title}
        </text>

        {/* === CỘT THÔNG TIN: SUẤT CHIẾU & PHÒNG (x=50 đến x=440) === */}
        {/* SUẤT CHIẾU */}
        <text
          x="50" y="176"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="11" fontWeight="600" fill="#60524B"
          letterSpacing="2.5"
        >
          SUẤT CHIẾU
        </text>
        <text
          x="50" y="204"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize="20" fontWeight="600" fill="#170C09"
        >
          {showtime}
        </text>

        {/* PHÒNG CHIẾU (nằm riêng dòng dưới suất chiếu, rộng rãi không va chạm) */}
        <text
          x="50" y="234"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="11" fontWeight="600" fill="#60524B"
          letterSpacing="2.5"
        >
          PHÒNG
        </text>
        <text
          x="50" y="262"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize={room.length > 22 ? "17" : "20"} fontWeight="600" fill="#170C09"
        >
          {room}
        </text>

        {/* === CỘT GHẾ: Khối riêng biệt (x=465 đến x=605) === */}
        <rect
          x="465" y="160"
          width="140" height="106"
          rx="4"
          fill="#F5EFE6"
          stroke="#170C09"
          strokeOpacity="0.12"
          strokeWidth="1.5"
        />
        <text
          x="535" y="186"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="11" fontWeight="700" fill="#60524B"
          textAnchor="middle" letterSpacing="2.5"
        >
          GHẾ
        </text>
        <text
          x="535" y="236"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize={seat.length > 6 ? "20" : seat.length > 3 ? "28" : "38"}
          fontWeight="800"
          fill="#970013"
          textAnchor="middle"
        >
          {seat}
        </text>

        {/* === Phân cách nét đứt === */}
        <path
          d="M50 286H605"
          stroke="#170C09"
          strokeOpacity="0.18"
          strokeDasharray="3 5"
          strokeWidth="2"
          strokeLinecap="round"
        />

        {/* === TỔNG TIỀN === */}
        <text
          x="50" y="328"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="11" fontWeight="600" fill="#60524B"
          letterSpacing="2.5"
        >
          TỔNG TIỀN
        </text>
        <text
          x="160" y="332"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize="30" fontWeight="700" fill="#970013"
        >
          {totalAmount}
        </text>

        {/* === CUỐNG VÉ ĐỎ: QR CODE & MÃ VÉ (x=640 đến x=920, tâm x=780) === */}
        <foreignObject x="676" y="46" width="208" height="208">
          <div
            // @ts-expect-error xmlns needed for SVG foreignObject
            xmlns="http://www.w3.org/1999/xhtml"
            style={{
              width: "100%",
              height: "100%",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              background: "#F9F5F0",
              padding: "10px",
              boxSizing: "border-box",
            }}
          >
            {isImageQr ? (
              <img
                src={qrValue}
                alt={`Mã QR vé ${ticketCode}`}
                style={{ width: "100%", height: "100%", objectFit: "contain" }}
              />
            ) : (
              <QRCodeSVG
                value={qrValue}
                size={188}
                bgColor="#F9F5F0"
                fgColor="#170C09"
                level="M"
              />
            )}
          </div>
        </foreignObject>

        {/* Mã vé */}
        <text
          x="780" y="290"
          fontFamily="'Archivo Variable', 'Arial Narrow', monospace"
          fontSize={ticketCode.length > 18 ? "13" : ticketCode.length > 14 ? "15" : "17"}
          fontWeight="700"
          fill="#F9F5F0"
          textAnchor="middle"
          letterSpacing="1"
        >
          {ticketCode}
        </text>
        {/* Hướng dẫn */}
        <text
          x="780" y="318"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="12" fontWeight="400" fill="#F9F5F0"
          textAnchor="middle" opacity="0.8"
        >
          Quét mã tại quầy để vào rạp
        </text>

        {/* === Overlay "ĐÃ SOÁT VÉ" khi isCheckedIn === */}
        {isCheckedIn && (
          <g>
            <rect x="10" y="10" width="630" height="360" fill="#170C09" fillOpacity="0.55" />
            <text
              x="325" y="210"
              fontFamily="'Archivo Variable', 'Arial Narrow', sans-serif"
              fontSize="56" fontWeight="800" fill="#FFFDFB"
              fillOpacity="0.9"
              textAnchor="middle"
              transform="rotate(-20, 325, 200)"
              letterSpacing="2"
            >
              ĐÃ SOÁT VÉ
            </text>
          </g>
        )}
      </svg>
    </div>
  );
};
