import { QRCodeSVG } from "qrcode.react";
import { cn } from "@/lib/utils";

interface PerfoTicketProps {
  /** Tên phim */
  movieTitle: string;
  /** Suất chiếu: e.g. "19:45 · 24/10/2026" */
  showtime: string;
  /** Phòng chiếu: e.g. "P03" */
  room: string;
  /** Ghế: e.g. "H12" hoặc "H12, H13" */
  seat: string;
  /** Tổng tiền: e.g. "120.000 ₫" */
  totalAmount: string;
  /** Mã vé: e.g. "PV-0042-H12" */
  ticketCode: string;
  /** QR value để generate mã QR */
  qrValue: string;
  /** Đã được soát vé chưa */
  isCheckedIn?: boolean;
  /** Extra class cho wrapper */
  className?: string;
}

/**
 * Perfo Ticket — React component dựng từ perfo-ticket-template.svg.
 *
 * Layout 920×380 gốc được giữ nguyên tỷ lệ qua viewBox.
 * - Nửa trái (thân vé cream): tên phim, suất chiếu, phòng, ghế, tổng tiền.
 * - Đường perforation (lỗ kim đục) dọc giữa.
 * - Nửa phải (cuống đỏ Perfo): mã QR, mã vé, hướng dẫn soát.
 * - Khi isCheckedIn=true: dải chéo "ĐÃ SOÁT" màu mờ phủ lên.
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
  // truncate long movie title
  const title = movieTitle.length > 22 ? movieTitle.slice(0, 21) + "…" : movieTitle;

  return (
    <div className={cn("w-full max-w-3xl", className)}>
      <svg
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 920 380"
        width="100%"
        role="img"
        aria-label={`Vé điện tử Perfo — ${movieTitle}`}
        className="block"
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
        {/* Cream background */}
        <path
          d="M18 10H618A22 22 0 0 0 662 10H902A8 8 0 0 1 910 18V362A8 8 0 0 1 902 370H662A22 22 0 0 0 618 370H18A8 8 0 0 1 10 362V18A8 8 0 0 1 18 10Z"
          fill="#FFFDFB"
          stroke="#170C09"
          strokeOpacity="0.18"
          strokeWidth="2"
        />
        {/* Red stub overlay */}
        <path
          d="M18 10H618A22 22 0 0 0 662 10H902A8 8 0 0 1 910 18V362A8 8 0 0 1 902 370H662A22 22 0 0 0 618 370H18A8 8 0 0 1 10 362V18A8 8 0 0 1 18 10Z"
          fill="#970013"
          clipPath="url(#stub-clip)"
        />

        {/* === Perforation dots === */}
        {[40, 64, 88, 112, 136, 160, 184, 208, 232, 256, 280, 304, 328].map((cy) => (
          <circle key={cy} cx="640" cy={cy} r="3.5" fill="#B97F12" />
        ))}

        {/* === Logo BrandMark (mini ticket icon) === */}
        <g transform="translate(46.8 28.8) scale(0.8)">
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
        <g transform="translate(106 40) scale(0.6)">
          <g clipPath="url(#wordmark-clip)" fill="none" stroke="#170C09" strokeWidth="9" strokeLinejoin="miter" strokeMiterlimit="10">
            <path transform="translate(0 0)" d="M4.5 48V4.5H23L27.5 9V24L23 28.5H4.5" />
            <path transform="translate(38 0)" d="M27.5 4.5H4.5V43.5H27.5M4.5 24H23" />
            <path transform="translate(76 0)" d="M4.5 48V4.5H23L27.5 9V24L23 28.5H4.5M14 28.5L30.3 52" />
            <path transform="translate(114 0)" d="M27.5 4.5H4.5V48M4.5 24H23" />
            <path transform="translate(152 0)" d="M9 4.5H23L27.5 9V39L23 43.5H9L4.5 39V9Z" />
          </g>
          <circle cx="168" cy="24" r="4.2" fill="#970013" />
        </g>

        {/* === Nội dung thân vé (trái) === */}
        {/* Label PHIM */}
        <text
          x="50" y="118"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="12" fontWeight="500" fill="#60524B"
          letterSpacing="2.5"
        >
          PHIM
        </text>
        {/* Tên phim */}
        <text
          x="50" y="158"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize="38" fontWeight="700" fill="#170C09"
          letterSpacing="0.5"
        >
          {title}
        </text>

        {/* Label SUẤT CHIẾU */}
        <text
          x="50" y="210"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="12" fontWeight="500" fill="#60524B"
          letterSpacing="2.5"
        >
          SUẤT CHIẾU
        </text>
        {/* Giờ chiếu */}
        <text
          x="50" y="250"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize="30" fontWeight="600" fill="#170C09"
        >
          {showtime}
        </text>

        {/* Label PHÒNG */}
        <text
          x="330" y="210"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="12" fontWeight="500" fill="#60524B"
          letterSpacing="2.5"
        >
          PHÒNG
        </text>
        <text
          x="330" y="250"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize="30" fontWeight="600" fill="#170C09"
        >
          {room}
        </text>

        {/* Label GHẾ */}
        <text
          x="450" y="210"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="12" fontWeight="500" fill="#60524B"
          letterSpacing="2.5"
        >
          GHẾ
        </text>
        <text
          x="450" y="250"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize="30" fontWeight="600" fill="#170C09"
        >
          {seat}
        </text>

        {/* Dashed divider */}
        <path
          d="M50 278H590"
          stroke="#170C09"
          strokeOpacity="0.2"
          strokeDasharray="2 6"
          strokeWidth="2"
          strokeLinecap="round"
        />

        {/* Label TỔNG TIỀN */}
        <text
          x="50" y="325"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="12" fontWeight="500" fill="#60524B"
          letterSpacing="2.5"
        >
          TỔNG TIỀN
        </text>
        {/* Amount */}
        <text
          x="180" y="330"
          fontFamily="'Archivo Variable', 'Arial Narrow', Impact, sans-serif"
          fontSize="34" fontWeight="600" fill="#970013"
        >
          {totalAmount}
        </text>

        {/* === QR Code (cuống đỏ) — embedded via foreignObject === */}
        <foreignObject x="671" y="50" width="208" height="208">
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
              padding: "8px",
            }}
          >
            <QRCodeSVG
              value={qrValue}
              size={188}
              bgColor="#F9F5F0"
              fgColor="#170C09"
              level="M"
            />
          </div>
        </foreignObject>

        {/* Ticket code */}
        <text
          x="775" y="292"
          fontFamily="'Archivo Variable', 'Arial Narrow', monospace"
          fontSize="18" fontWeight="600" fill="#F9F5F0"
          textAnchor="middle" letterSpacing="1.5"
        >
          {ticketCode}
        </text>
        {/* Hướng dẫn */}
        <text
          x="775" y="320"
          fontFamily="'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif"
          fontSize="12" fontWeight="400" fill="#F9F5F0"
          textAnchor="middle" opacity="0.75"
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
