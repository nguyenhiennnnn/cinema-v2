import { useNavigate } from "react-router";

import { Input } from "@/components/ui/input";
import { PageHeader } from "@/components/common/page-header";
import { DataTable } from "@/components/common/data-table/data-table";

import { useAdminBookingParams } from "@/hooks/use-query-params";
import { useAdminBookings } from "../hooks/use-admin-booking";
import type { AdminBookingListItemDto } from "../types";
import { createBookingColumns } from "../components/booking-columns";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { stripNulls } from "@/lib/query-params";
import { DEFAULT_PAGE } from "@/config/default-constants";
import { DateFilterPicker } from "@/components/common/filter/date-filter-picker";
import { bookingStatusOptions } from "@/lib/enum-options";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";

const AdminBookingsPage = () => {
  const [params, setParams] = useAdminBookingParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);
  const navigate = useNavigate();

  const bookings = useAdminBookings(stripNulls(params));

  const handleViewDetail = (booking: AdminBookingListItemDto) => {
    navigate(`/admin/bookings/${booking.bookingId}`);
  };

  const columns = createBookingColumns({ onViewDetail: handleViewDetail });

  return (
    <div>
      <PageHeader
        title="Đặt vé"
        description="Theo dõi đơn đặt vé, kênh bán và trạng thái xử lý."
      />

      <div className="mb-4 grid grid-cols-2 gap-3 md:grid-cols-5">
        <Input
          placeholder="Tìm theo mã đơn, khách hàng..."
          value={params.search}
          onChange={(e) => setParams({ search: e.target.value, page: DEFAULT_PAGE })}
          className="md:col-span-2"
        />
        <EnumSelectFilter
          value={params.status}
          onChange={(status) => setParams({ status, page: DEFAULT_PAGE })}
          options={bookingStatusOptions}
          placeholder="Trạng thái"
          allLabel="Tất cả trạng thái"
        />
        <DateFilterPicker
          value={params.fromDate}
          onChange={(fromDate) => setParams({ fromDate, page: DEFAULT_PAGE })}
          placeholder="Từ ngày"
        />
        <DateFilterPicker
          value={params.toDate}
          onChange={(toDate) => setParams({ toDate, page: DEFAULT_PAGE })}
          placeholder="Đến ngày"
        />
      </div>

      <DataTable
        columns={columns}
        data={bookings.data?.items ?? []}
        isLoading={bookings.isLoading}
        totalItems={bookings.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        emptyMessage="Chưa có đơn đặt vé nào."
      />
    </div>
  );
};

export default AdminBookingsPage;