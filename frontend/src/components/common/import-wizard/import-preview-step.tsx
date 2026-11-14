import { useState } from "react";
import { ArrowLeft, Download } from "lucide-react";
import { useTable, type ColumnDef, type PaginationState } from "@tanstack/react-table";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Spinner } from "@/components/common/spinner";
import { DataTablePagination } from "@/components/common/data-table/data-table-pagination";
import {
  features,
  type DataTableFeatures,
} from "@/components/common/data-table/data-table-features";
import { IMPORT_PREVIEW_PAGE_SIZE } from "@/config/default-constants";
import { useBlockDialogClose } from "@/hooks/use-block-dialog-close";
import { extractApiError } from "@/lib/error";
import { cn } from "@/lib/utils";
import type {
  ImportApplyOutcome,
  ImportPreviewResponse,
  ImportPreviewRow,
} from "@/types/import";

import { ImportPreviewCell, ImportRowStatus } from "./import-preview-cell";
import { ImportWizardFooter } from "./import-wizard-footer";
import type { ImportWizardConfig } from "./import-wizard-types";
import { useImportApply, useImportErrorFile } from "./use-import-wizard";

const ROW_NUMBER_COLUMN_ID = "rowNumber";
const STATUS_COLUMN_ID = "status";
const NOT_FOUND_STATUS = 404;

type PreviewColumn = ColumnDef<DataTableFeatures, ImportPreviewRow>;

const buildColumns = (config: ImportWizardConfig): PreviewColumn[] => [
  {
    id: ROW_NUMBER_COLUMN_ID,
    header: "Dòng",
    cell: ({ row }) => (
      <span className="text-muted-foreground tabular-nums">
        #{row.original.rowNumber}
      </span>
    ),
  },
  ...config.columns.map(
    (column): PreviewColumn => ({
      id: column.key,
      header: column.header,
      cell: ({ row }) => <ImportPreviewCell row={row.original} column={column} />,
    }),
  ),
  {
    id: STATUS_COLUMN_ID,
    header: "Trạng thái",
    cell: ({ row }) => <ImportRowStatus errors={row.original.errors} />,
  },
];

interface ImportPreviewStepProps {
  config: ImportWizardConfig;
  preview: ImportPreviewResponse;
  onApplied: (outcome: ImportApplyOutcome) => void;
  onExpired: () => void;
  onBack: () => void;
}

export const ImportPreviewStep = ({
  config,
  preview,
  onApplied,
  onExpired,
  onBack,
}: ImportPreviewStepProps) => {
  const [onlyErrors, setOnlyErrors] = useState(false);
  const [skipInvalidRows, setSkipInvalidRows] = useState(false);
  const [pagination, setPagination] = useState<PaginationState>({
    pageIndex: 0,
    pageSize: IMPORT_PREVIEW_PAGE_SIZE,
  });

  const apply = useImportApply(config);
  const errorFile = useImportErrorFile(config, preview.previewId);
  useBlockDialogClose(apply.isPending);

  const hasInvalidRows = preview.invalidRows > 0;
  const rows = onlyErrors
    ? preview.rows.filter((row) => row.errors.length > 0)
    : preview.rows;

  const table = useTable({
    features,
    data: rows,
    columns: buildColumns(config),
    state: { pagination },
    onPaginationChange: setPagination,
  });

  const canApply =
    preview.validRows > 0 && (!hasInvalidRows || skipInvalidRows) && !apply.isPending;

  const handleOnlyErrorsChange = (checked: boolean) => {
    setOnlyErrors(checked);
    setPagination((prev) => ({ ...prev, pageIndex: 0 }));
  };

  const handleApply = () => {
    apply.mutate(
      { previewId: preview.previewId, skipInvalidRows },
      {
        onSuccess: onApplied,
        onError: (error) => {
          if (extractApiError(error).statusCode === NOT_FOUND_STATUS) onExpired();
        },
      },
    );
  };

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center gap-2">
        <Badge variant="secondary">Tổng {preview.totalRows} dòng</Badge>
        <Badge variant="success">{preview.validRows} hợp lệ</Badge>
        <Badge variant={hasInvalidRows ? "destructive" : "secondary"}>
          {preview.invalidRows} lỗi
        </Badge>
      </div>

      {hasInvalidRows && (
        <div className="flex flex-col gap-3 border border-border p-3 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:gap-6">
            <label className="flex cursor-pointer items-center gap-2 text-sm">
              <Checkbox
                checked={onlyErrors}
                onCheckedChange={(checked) => handleOnlyErrorsChange(!!checked)}
              />
              <span>Chỉ hiện dòng lỗi</span>
            </label>
            <label className="flex cursor-pointer items-center gap-2 text-sm">
              <Checkbox
                checked={skipInvalidRows}
                onCheckedChange={(checked) => setSkipInvalidRows(!!checked)}
                disabled={apply.isPending}
              />
              <span>Bỏ qua dòng lỗi</span>
            </label>
          </div>
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="shrink-0 gap-2"
            onClick={() => errorFile.mutate()}
            disabled={errorFile.isPending}
          >
            {errorFile.isPending ? <Spinner size="sm" decorative /> : <Download />}
            <span>Tải file lỗi</span>
          </Button>
        </div>
      )}

      <div className="max-h-[50vh] overflow-auto border">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <TableHead key={header.id} className="whitespace-nowrap">
                    {header.isPlaceholder ? null : (
                      <table.FlexRender header={header} />
                    )}
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow
                  key={row.id}
                  className={cn(row.original.errors.length > 0 && "bg-destructive/5")}
                >
                  {row.getVisibleCells().map((cell) => (
                    <TableCell
                      key={cell.id}
                      className={cn(
                        row.original.errors.some(
                          (error) => error.column === cell.column.id,
                        ) && "bg-destructive/15",
                      )}
                    >
                      <table.FlexRender cell={cell} />
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={config.columns.length + 2}
                  className="h-24 text-center text-muted-foreground"
                >
                  Không có dòng nào.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>

      <DataTablePagination table={table} totalItems={rows.length} />

      {hasInvalidRows && !skipInvalidRows && (
        <p className="text-sm text-muted-foreground">
          Còn {preview.invalidRows} dòng lỗi. Hãy sửa file hoặc chọn &quot;Bỏ qua dòng
          lỗi&quot; để chỉ nhập các dòng hợp lệ.
        </p>
      )}

      <ImportWizardFooter>
        <Button
          type="button"
          variant="outline"
          onClick={onBack}
          disabled={apply.isPending}
        >
          <ArrowLeft />
          Chọn file khác
        </Button>
        <Button type="button" onClick={handleApply} disabled={!canApply}>
          {apply.isPending && <Spinner size="sm" decorative className="text-current" />}
          {apply.isPending
            ? "Đang nhập..."
            : `Nhập ${preview.validRows} ${config.itemLabel}`}
        </Button>
      </ImportWizardFooter>
    </div>
  );
};
