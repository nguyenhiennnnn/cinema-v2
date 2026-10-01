import {
  Award,
  Sparkles,
  Crown,
  Gem,
  Percent,
  Gift,
  CalendarCheck,
} from "lucide-react";

export const TIER_VISUALS = [
  {
    icon: Award,
    accent: "text-warning",
    ring: "hover:border-warning/50",
    perk: "Quyền lợi hội viên cơ bản",
  },
  {
    icon: Sparkles,
    accent: "text-muted-foreground",
    ring: "hover:border-muted-foreground/40",
    perk: "Chiết khấu ưu đãi vé & bắp nước",
  },
  {
    icon: Crown,
    accent: "text-accent",
    ring: "hover:border-accent/50",
    perk: "Ưu tiên giữ chỗ sự kiện đặc biệt",
  },
  {
    icon: Gem,
    accent: "text-primary",
    ring: "hover:border-primary/50",
    perk: "Tặng combo bắp nước miễn phí dịp sinh nhật",
  },
] as const;

export const COMPARE_ROWS = [
  { label: "Tích điểm mỗi giao dịch", unlockAt: 0, icon: Sparkles },
  { label: "Chiết khấu hóa đơn vé & bắp nước", unlockAt: 1, icon: Percent },
  { label: "Ưu tiên giữ chỗ sự kiện", unlockAt: 2, icon: CalendarCheck },
  { label: "Combo bắp nước miễn phí sinh nhật", unlockAt: 3, icon: Gift },
] as const;

export const FAQ_ITEMS = [
  {
    question: "Làm sao để tích điểm hội viên?",
    answer:
      "Mỗi 10.000 VNĐ chi tiêu cho vé xem phim hoặc bắp nước tại rạp (thanh toán thành công) sẽ được cộng 1 điểm thưởng vào tài khoản của bạn. Điểm được cộng tự động ngay sau khi đơn hàng hoàn tất.",
  },
  {
    question: "Hạng thẻ của tôi có bị hết hạn không?",
    answer:
      "Hạng thẻ được xét lại theo tổng điểm tích lũy trong năm dương lịch. Nếu không đạt đủ điểm duy trì hạng vào cuối kỳ đánh giá, tài khoản sẽ tự động điều chỉnh về hạng phù hợp với điểm hiện có.",
  },
  {
    question: "Hủy vé thì điểm thưởng đã tích có bị trừ không?",
    answer:
      "Khi vé được hủy và hoàn tiền thành công, số điểm thưởng tương ứng với giá trị đơn hàng đó sẽ được trừ lại khỏi tổng điểm tích lũy của bạn.",
  },
];
