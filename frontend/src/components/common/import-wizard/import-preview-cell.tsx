import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { Badge } from "@/components/ui/badge";
import type { ImportCellError, ImportPreviewRow } from "@/types/import";

import type { ImportColumnConfig } from "./import-wizard-types";

interface ImportPreviewCellProps {
  row: ImportPreviewRow;
  column: ImportColumnConfig;
}

export const ImportPreviewCell = ({ row, column }: ImportPreviewCellProps) => {
  const value = row.data[column.key];
  const cellErrors = row.errors.filter((error) => error.column === column.key);

  if (cellErrors.length === 0) {
    return <span className="block max-w-56 truncate">{value ?? ""}</span>;
  }

  return (
    <Tooltip>
      <TooltipTrigger
        type="button"
        className="block max-w-56 cursor-help truncate text-left font-medium text-destructive"
      >
        {value || "(trống)"}
      </TooltipTrigger>
      <TooltipContent>
        <ul className="space-y-0.5">
          {cellErrors.map((error) => (
            <li key={error.message}>{error.message}</li>
          ))}
        </ul>
      </TooltipContent>
    </Tooltip>
  );
};

interface ImportRowStatusProps {
  errors: ImportCellError[];
}

export const ImportRowStatus = ({ errors }: ImportRowStatusProps) => {
  if (errors.length === 0) return <Badge variant="success">Hợp lệ</Badge>;

  return (
    <Tooltip>
      <TooltipTrigger type="button" className="cursor-help">
        <Badge variant="destructive">{errors.length} lỗi</Badge>
      </TooltipTrigger>
      <TooltipContent>
        <ul className="space-y-0.5">
          {errors.map((error) => (
            <li key={`${error.column}-${error.message}`}>
              {error.column}: {error.message}
            </li>
          ))}
        </ul>
      </TooltipContent>
    </Tooltip>
  );
};
