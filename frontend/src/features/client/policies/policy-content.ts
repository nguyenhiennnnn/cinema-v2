import { BRAND } from "@/config/brand";

export interface PolicyContext {
  cancelDeadlineHours?: number;
  cancelRefundPercent?: number;
  stopSellingMinutesBefore?: number;
}

export interface PolicySection {
  heading: string;
  body: string[];
  list?: boolean;
}

export interface PolicyDoc {
  slug: string;
  title: string;
  description: string;
  updatedAt: string;
  sections: (ctx: PolicyContext) => PolicySection[];
}

const name = BRAND.name;

export const POLICIES: PolicyDoc[] = [
  {
    slug: "terms",
    title: "Điều khoản sử dụng",
    description: `Điều khoản sử dụng dịch vụ đặt vé xem phim trực tuyến của ${name}.`,
    updatedAt: "03/11/2026",
    sections: () => [
      {
        heading: "1. Chấp nhận điều khoản",
        body: [
          `Khi truy cập website hoặc đặt vé tại ${name}, bạn đồng ý tuân thủ các điều khoản dưới đây. Nếu không đồng ý, vui lòng ngừng sử dụng dịch vụ.`,
        ],
      },
      {
        heading: "2. Tài khoản",
        list: true,
        body: [
          "Bạn cần cung cấp thông tin chính xác khi đăng ký và chịu trách nhiệm bảo mật mật khẩu của mình.",
          "Mỗi người nên sử dụng một tài khoản; không chia sẻ tài khoản cho người khác.",
          `${name} có quyền tạm khóa tài khoản có dấu hiệu gian lận hoặc vi phạm điều khoản.`,
        ],
      },
      {
        heading: "3. Đặt vé và thanh toán",
        list: true,
        body: [
          "Ghế được giữ tạm thời trong thời gian đếm ngược trên màn hình thanh toán; hết thời gian, ghế sẽ được nhả tự động.",
          "Đơn đặt vé chỉ có hiệu lực khi thanh toán thành công qua các cổng thanh toán được hỗ trợ.",
          "Giá vé, combo và ưu đãi hiển thị tại thời điểm thanh toán là giá cuối cùng áp dụng cho đơn đặt vé.",
        ],
      },
      {
        heading: "4. Vé điện tử",
        body: [
          "Mỗi vé có mã QR duy nhất dùng để check-in tại rạp. Vui lòng không chia sẻ mã QR cho người lạ; vé đã được quét sẽ không thể sử dụng lại.",
        ],
      },
      {
        heading: "5. Điểm thưởng thành viên",
        body: [
          `Điểm thưởng ${name} Rewards được tích lũy theo chi tiêu thực tế và quy đổi hạng thẻ theo chương trình thành viên hiện hành. Điểm không có giá trị quy đổi thành tiền mặt.`,
        ],
      },
      {
        heading: "6. Hành vi bị nghiêm cấm",
        list: true,
        body: [
          "Sử dụng phần mềm tự động để đặt giữ ghế hàng loạt hoặc gây quá tải hệ thống.",
          "Mua bán lại vé nhằm mục đích thu lợi.",
          "Can thiệp trái phép vào hệ thống, dữ liệu hoặc tài khoản của người khác.",
        ],
      },
      {
        heading: "7. Thay đổi điều khoản",
        body: [
          `${name} có thể cập nhật điều khoản theo từng thời điểm. Phiên bản mới có hiệu lực kể từ khi được đăng tải trên website.`,
        ],
      },
      {
        heading: "8. Liên hệ",
        body: [
          `Mọi thắc mắc xin liên hệ ${BRAND.supportEmail} hoặc hotline ${BRAND.supportHotline}.`,
        ],
      },
    ],
  },
  {
    slug: "privacy",
    title: "Chính sách bảo mật",
    description: `Cách ${name} thu thập, sử dụng và bảo vệ dữ liệu cá nhân của khách hàng.`,
    updatedAt: "03/11/2026",
    sections: () => [
      {
        heading: "1. Dữ liệu chúng tôi thu thập",
        list: true,
        body: [
          "Thông tin tài khoản: họ tên, email, số điện thoại, ngày sinh (nếu bạn cung cấp).",
          "Thông tin giao dịch: lịch sử đặt vé, combo, phương thức và trạng thái thanh toán.",
          "Dữ liệu kỹ thuật: địa chỉ IP, loại thiết bị, trình duyệt, nhật ký truy cập phục vụ bảo mật.",
          "Đăng nhập bằng Google: chúng tôi chỉ nhận họ tên, email và ảnh đại diện từ tài khoản Google của bạn.",
        ],
      },
      {
        heading: "2. Mục đích sử dụng",
        list: true,
        body: [
          "Xử lý đặt vé, thanh toán, hoàn tiền và gửi vé điện tử qua email.",
          "Tích điểm và quản lý hạng thành viên.",
          "Thông báo thay đổi suất chiếu, hủy suất hoặc các sự cố liên quan đến đơn đặt vé.",
          "Phòng chống gian lận và cải thiện chất lượng dịch vụ.",
        ],
      },
      {
        heading: "3. Chia sẻ dữ liệu",
        body: [
          `${name} không bán dữ liệu cá nhân. Dữ liệu chỉ được chia sẻ với đối tác cần thiết để vận hành dịch vụ như cổng thanh toán (VNPay, MoMo, Stripe), dịch vụ gửi email và lưu trữ hình ảnh, hoặc theo yêu cầu hợp pháp của cơ quan có thẩm quyền.`,
        ],
      },
      {
        heading: "4. Thông tin thanh toán",
        body: [
          `${name} không lưu số thẻ hay thông tin xác thực ngân hàng của bạn. Mọi giao dịch được xử lý trực tiếp bởi cổng thanh toán.`,
        ],
      },
      {
        heading: "5. Lưu trữ và bảo mật",
        body: [
          "Mật khẩu được mã hóa một chiều, kết nối được bảo vệ bằng HTTPS và quyền truy cập dữ liệu được phân quyền theo vai trò. Dữ liệu được lưu trong thời gian cần thiết cho mục đích nêu trên hoặc theo quy định pháp luật.",
        ],
      },
      {
        heading: "6. Quyền của bạn",
        list: true,
        body: [
          "Xem và cập nhật thông tin cá nhân tại trang Hồ sơ.",
          "Đổi mật khẩu bất kỳ lúc nào.",
          `Yêu cầu truy xuất hoặc xóa dữ liệu bằng cách gửi email tới ${BRAND.supportEmail}.`,
        ],
      },
      {
        heading: "7. Cookie và lưu trữ cục bộ",
        body: [
          "Chúng tôi lưu mã làm mới phiên đăng nhập và tùy chọn giao diện (sáng/tối) trên trình duyệt của bạn để duy trì trải nghiệm. Bạn có thể xóa chúng bất kỳ lúc nào trong cài đặt trình duyệt.",
        ],
      },
    ],
  },
  {
    slug: "refund",
    title: "Quy định hoàn/đổi vé",
    description: `Điều kiện hủy, hoàn tiền và đổi vé khi đặt vé xem phim tại ${name}.`,
    updatedAt: "03/11/2026",
    sections: (ctx) => {
      const hours = ctx.cancelDeadlineHours;
      const percent = ctx.cancelRefundPercent;
      return [
        {
          heading: "1. Hủy vé do khách hàng",
          list: true,
          body: [
            hours != null
              ? `Bạn có thể hủy đơn đặt vé đã thanh toán trước giờ chiếu ít nhất ${hours} giờ.`
              : "Bạn có thể hủy đơn đặt vé đã thanh toán trước giờ chiếu theo thời hạn rạp quy định.",
            percent != null
              ? `Số tiền hoàn lại là ${percent}% giá trị đơn đặt vé.`
              : "Số tiền hoàn lại tính theo tỷ lệ phần trăm rạp quy định tại thời điểm hủy.",
            "Quá thời hạn trên, hệ thống sẽ không cho phép hủy hoặc hoàn tiền.",
            "Vé đã check-in tại rạp không thể hủy.",
          ],
        },
        {
          heading: "2. Suất chiếu bị hủy hoặc không xuất được vé",
          body: [
            `Nếu suất chiếu bị rạp hủy, hoặc thanh toán thành công nhưng ${name} không xuất được vé, bạn được hoàn 100% giá trị đơn. Hệ thống sẽ tự động hoàn về phương thức thanh toán ban đầu khi cổng thanh toán hỗ trợ; trường hợp cần xử lý thủ công, nhân viên sẽ liên hệ qua email.`,
          ],
        },
        {
          heading: "3. Thời gian hoàn tiền",
          body: [
            "Thời gian tiền về tài khoản phụ thuộc cổng thanh toán và ngân hàng, thường từ 3–15 ngày làm việc kể từ khi yêu cầu hoàn được chấp nhận. Bạn sẽ nhận email xác nhận khi hoàn tiền được thực hiện.",
          ],
        },
        {
          heading: "4. Đổi vé",
          body: [
            "Hiện chưa hỗ trợ đổi suất hoặc đổi ghế trực tiếp. Nếu muốn thay đổi, vui lòng hủy đơn (nếu còn trong thời hạn hủy) và đặt lại đơn mới.",
          ],
        },
        {
          heading: "5. Điểm thưởng và ưu đãi",
          body: [
            "Điểm thưởng tích lũy từ đơn bị hủy sẽ được điều chỉnh tương ứng. Ưu đãi đã áp dụng cho đơn bị hủy có thể không được hoàn lại.",
          ],
        },
        {
          heading: "6. Hỗ trợ",
          body: [
            `Cần trợ giúp về hoàn tiền, liên hệ ${BRAND.supportEmail} hoặc hotline ${BRAND.supportHotline} kèm mã đơn đặt vé.`,
          ],
        },
      ];
    },
  },
  {
    slug: "age-rating",
    title: "Quy định độ tuổi",
    description: `Quy định phân loại độ tuổi khán giả khi xem phim tại ${name}.`,
    updatedAt: "03/11/2026",
    sections: (ctx) => [
      {
        heading: "1. Phân loại độ tuổi",
        body: [
          `Mỗi phim tại ${name} được gắn nhãn độ tuổi theo quy định của cơ quan quản lý điện ảnh Việt Nam. Nhãn này hiển thị trên trang chi tiết phim và trên vé điện tử.`,
        ],
      },
      {
        heading: "2. Các nhãn thường gặp",
        list: true,
        body: [
          "P: phim phù hợp với mọi đối tượng khán giả.",
          "K: phim dành cho khán giả dưới 13 tuổi và có người bảo hộ đi cùng.",
          "T13: phim dành cho khán giả từ đủ 13 tuổi trở lên.",
          "T16: phim dành cho khán giả từ đủ 16 tuổi trở lên.",
          "T18: phim dành cho khán giả từ đủ 18 tuổi trở lên.",
          "C: phim không được phép phổ biến.",
        ],
      },
      {
        heading: "3. Kiểm tra tại rạp",
        body: [
          "Nhân viên có thể yêu cầu xuất trình giấy tờ tùy thân có ngày sinh (CCCD, hộ chiếu, thẻ học sinh/sinh viên) khi xem phim có giới hạn độ tuổi. Khách không đủ tuổi sẽ bị từ chối vào phòng chiếu.",
        ],
      },
      {
        heading: "4. Trường hợp bị từ chối",
        body: [
          ctx.cancelRefundPercent != null
            ? `Vé không được hoàn nếu khách không đáp ứng điều kiện độ tuổi tại thời điểm vào rạp. Vui lòng kiểm tra nhãn phim trước khi đặt; nếu hủy sớm trong thời hạn quy định, bạn vẫn có thể được hoàn ${ctx.cancelRefundPercent}% giá trị đơn.`
            : "Vé không được hoàn nếu khách không đáp ứng điều kiện độ tuổi tại thời điểm vào rạp. Vui lòng kiểm tra nhãn phim trước khi đặt.",
        ],
      },
    ],
  },
];

export const getPolicy = (slug?: string) =>
  POLICIES.find((policy) => policy.slug === slug);
