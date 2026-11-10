import { Input } from "@/components/ui/input";
import { PageHeader } from "@/components/common/page-header";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogOpen } from "@/stores/dialog-store";

import { useAdminPaymentParams } from "@/hooks/use-query-params";
import { useAdminPayments } from "../hooks/use-admin-payments";
import type { AdminPaymentListItemDto } from "../types";
import { createPaymentColumns } from "../components/payment-columns";
import { PaymentDetailView } from "../components/payment-detail-view";
import { useTablePagination } from "@/hooks/use-table-pagination";
import { stripNulls } from "@/lib/query-params";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";
import { paymentMethodOptions, paymentStatusOptions } from "@/lib/enum-options";
import { DateFilterPicker } from "@/components/common/filter/date-filter-picker";
import { DEFAULT_PAGE } from "@/config/default-constants";
import { useDebouncedValue } from "@/hooks/use-debounced-value";

const AdminPaymentsPage = () => {
  const [params, setParams] = useAdminPaymentParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const payments = useAdminPayments(stripNulls({ ...params, search: useDebouncedValue(params.search) }));
  const openDialog = useDialogOpen();

  const handleViewDetail = (payment: AdminPaymentListItemDto) => {
    openDialog({
      title: `Giao dịch #${payment.paymentId}`,
      description: `Chi tiết thanh toán cho đơn ${payment.bookingCode}.`,
      content: <PaymentDetailView paymentId={payment.paymentId} />,
    });
  };

  const columns = createPaymentColumns({ onViewDetail: handleViewDetail });

  return (
    <div>
      <PageHeader
        title="Thanh toán"
        description="Theo dõi giao dịch, trạng thái thanh toán và hoàn tiền."
      />

      <div className="mb-4 grid grid-cols-2 sm:grid-cols-3 gap-3 md:grid-cols-5">
        <Input
          placeholder="Tìm theo mã đơn..."
          value={params.search}
          onChange={(e) => setParams({ search: e.target.value, page: DEFAULT_PAGE })}
        />
        <EnumSelectFilter
          value={params.status}
          onChange={(status) => setParams({ status, page: DEFAULT_PAGE })}
          options={paymentStatusOptions}
          placeholder="Trạng thái"
          allLabel="Tất cả trạng thái"
        />
        <EnumSelectFilter
          value={params.paymentMethod}
          onChange={(paymentMethod) => setParams({ paymentMethod, page: DEFAULT_PAGE })}
          options={paymentMethodOptions}
          placeholder="Phương thức"
          allLabel="Tất cả phương thức"
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
        data={payments.data?.items ?? []}
        isLoading={payments.isLoading}
        totalItems={payments.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        emptyMessage="Chưa có giao dịch nào."
      />
    </div>
  );
};

export default AdminPaymentsPage;