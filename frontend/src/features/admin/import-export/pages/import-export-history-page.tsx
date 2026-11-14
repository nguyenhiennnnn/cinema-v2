import { PageHeader } from "@/components/common/page-header";
import { DataTable } from "@/components/common/data-table/data-table";
import { EnumSelectFilter } from "@/components/common/filter/enum-select-filter";
import { DEFAULT_PAGE } from "@/config/default-constants";
import { useAdminImportExportJobParams } from "@/hooks/use-query-params";
import { useTablePagination } from "@/hooks/use-table-pagination";
import {
  importExportJobStatusOptions,
  importExportJobTypeOptions,
  importExportModuleOptions,
} from "@/lib/enum-options";
import { stripNulls } from "@/lib/query-params";
import { useAuthUser } from "@/stores/auth-store";

import { createImportExportJobColumns } from "../components/import-export-job-columns";
import { useImportExportJobDetailDialog } from "../hooks/use-import-export-job-detail-dialog";
import {
  useDownloadImportExportJob,
  useImportExportJobs,
} from "../hooks/use-import-export-jobs";

const AdminImportExportHistoryPage = () => {
  const [params, setParams] = useAdminImportExportJobParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);

  const jobs = useImportExportJobs(stripNulls(params));
  const user = useAuthUser();
  const download = useDownloadImportExportJob();
  const openDetail = useImportExportJobDetailDialog();

  const columns = createImportExportJobColumns({
    currentUserId: user?.userId,
    downloadingJobId: download.isPending ? (download.variables?.jobId ?? null) : null,
    onViewDetail: (job) => openDetail(job.jobId),
    onDownload: (job) =>
      download.mutate({
        jobId: job.jobId,
        fallbackFilename: job.fileName ?? `${job.module}.${job.format}`,
      }),
  });

  return (
    <div>
      <PageHeader
        title="Lịch sử Import/Export"
        description="Theo dõi tiến độ, kết quả và tải lại file của các tác vụ nhập/xuất dữ liệu."
      />

      <div className="mb-4 grid grid-cols-1 gap-3 sm:grid-cols-3">
        <EnumSelectFilter
          value={params.type}
          onChange={(type) => setParams({ type, page: DEFAULT_PAGE })}
          options={importExportJobTypeOptions}
          placeholder="Loại tác vụ"
          allLabel="Tất cả loại"
        />
        <EnumSelectFilter
          value={params.module}
          onChange={(module) => setParams({ module, page: DEFAULT_PAGE })}
          options={importExportModuleOptions}
          placeholder="Module"
          allLabel="Tất cả module"
        />
        <EnumSelectFilter
          value={params.status}
          onChange={(status) => setParams({ status, page: DEFAULT_PAGE })}
          options={importExportJobStatusOptions}
          placeholder="Trạng thái"
          allLabel="Tất cả trạng thái"
        />
      </div>

      <DataTable
        columns={columns}
        data={jobs.data?.items ?? []}
        isLoading={jobs.isLoading}
        isRefreshing={jobs.isPlaceholderData}
        totalItems={jobs.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        emptyMessage="Chưa có tác vụ nhập/xuất nào."
      />
    </div>
  );
};

export default AdminImportExportHistoryPage;
