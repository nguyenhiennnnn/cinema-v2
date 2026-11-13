import { useMemo, useState } from "react";
import { ArrowLeft } from "lucide-react";
import { useNavigate, useParams } from "react-router";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { toast } from "@/components/ui/toast";

import {
  useAdminRoom,
  useAdminRoomSeats,
  useAdminUpdateSeatLayout,
} from "../hooks/use-admin-rooms";
import { useAdminSeatTypes } from "@/features/admin/catalog/hooks/use-admin-seat-type";
import { resolveSeatVisualKind, deriveGridDimensions } from "@/components/common/seat-map/seat-map-types";
import { useSeatEditorState } from "../hooks/use-seat-editor-state";
import { SeatEditorToolbar } from "../components/seat-editor-toolbar";
import { SeatEditorGrid } from "../components/seat-editor-grid";
import { SeatEditorValidationPanel } from "../components/seat-editor-validation-panel";
import { SeatEditorSelectionBar } from "../components/seat-editor-selection-bar";
import { SeatEditorStats } from "../components/seat-editor-stats";
import { useSeatEditorShortcuts } from "../hooks/use-seat-editor-shortcuts";
import { useUnsavedChangesGuard } from "../hooks/use-unsaved-changes-guard";
import { describeBrush } from "../utils/seat-editor-labels";
import type { AdminRoomDto, SeatDto } from "../types";
import type { SeatTypeDto } from "@/features/admin/catalog/types";
import type { SeatEditorBrush } from "../types/seat-editor";
import { ROUTES } from "@/config/routes";

const SeatEditorReady = ({
  room,
  seats,
  seatTypes,
}: {
  room: AdminRoomDto;
  seats: SeatDto[];
  seatTypes: SeatTypeDto[];
}) => {
  const navigate = useNavigate();
  const updateSeatLayout = useAdminUpdateSeatLayout(room.roomId);
  const [brush, setBrush] = useState<SeatEditorBrush>({ type: "toggleActive" });

  const [previewMode, setPreviewMode] = useState(false);
  const [selectionMode, setSelectionMode] = useState(false);
  const [isPickingCopySource, setIsPickingCopySource] = useState(false);
  const [validationOpen, setValidationOpen] = useState(false);
  const [focusedIssue, setFocusedIssue] = useState<number | null>(null);

  const defaultSeatTypeId = useMemo(() => {
    if (seatTypes.length === 0) return null;
    const standard = seatTypes.find((t) => resolveSeatVisualKind(t.name) === "standard");
    return (standard ?? seatTypes[0]).seatTypeId;
  }, [seatTypes]);

  const { rows: initialRows, cols: initialCols } = useMemo(
    () => deriveGridDimensions(seats, room.rows, room.cols),
    [seats, room.rows, room.cols],
  );

  const editor = useSeatEditorState({
    initialRows,
    initialCols,
    initialSeats: seats,
    defaultSeatTypeId,
  });

  useUnsavedChangesGuard(editor.isDirty);

  const validationIssues = validationOpen ? editor.validate() : null;
  const focusedIssueIndex =
    validationIssues && focusedIssue !== null && validationIssues[focusedIssue] ? focusedIssue : null;

  const issueKeys = new Set<string>();
  if (validationIssues) {
    const visible = focusedIssueIndex !== null ? [validationIssues[focusedIssueIndex]] : validationIssues;
    visible.forEach((issue) => issue.cellKeys?.forEach((key) => issueKeys.add(key)));
  }

  const closeValidation = () => {
    setValidationOpen(false);
    setFocusedIssue(null);
  };

  const handleCellClick = (rowLabel: string, col: number) => {
    const warning = editor.applyBrush(rowLabel, col, brush);
    if (warning) toast.add({ title: warning, type: "warning" });
  };

  const handleSave = async () => {
    await updateSeatLayout.mutateAsync(editor.buildPayload(), {
      onSuccess: () => {
        editor.markSaved();
      }
    });
  };

  const handleTogglePreview = () => {
    if (!previewMode) {
      setSelectionMode(false);
      setIsPickingCopySource(false);
      editor.cancelCopyRow();
      editor.clearSelection();
      closeValidation();
    }
    setPreviewMode((v) => !v);
  };

  const handleToggleSelectionMode = () => {
    if (selectionMode) editor.clearSelection();
    setIsPickingCopySource(false);
    editor.cancelCopyRow();
    setSelectionMode((v) => !v);
  };

  const handleApplyToSelection = () => {
    const warning = editor.applyBrushToSelection(brush);
    if (warning) toast.add({ title: warning, type: "warning" });
  };

  const handleStartCopyRow = () => {
    setSelectionMode(false);
    editor.clearSelection();
    setIsPickingCopySource(true);
  };

  const handleCancelCopyRow = () => {
    setIsPickingCopySource(false);
    editor.cancelCopyRow();
  };

  const handleRowLabelClick = (rowLabel: string) => {
    if (isPickingCopySource) {
      editor.copyRow(rowLabel);
      setIsPickingCopySource(false);
      toast.add({ title: `Đã sao chép hàng ${rowLabel}.`, type: "success" });
      return;
    }
    if (editor.copiedRow) {
      const warning = editor.pasteRow(rowLabel);
      if (warning) {
        toast.add({ title: warning, type: "warning" });
      } else {
        toast.add({
          title: `Đã dán hàng ${editor.copiedRow.sourceRow} vào hàng ${rowLabel}.`,
          type: "success",
        });
      }
      return;
    }
    setSelectionMode(true);
    editor.selectRow(rowLabel);
  };

  const handleColLabelClick = (col: number) => {
    if (isPickingCopySource || editor.copiedRow) return;
    setSelectionMode(true);
    editor.selectCol(col);
  };

  const handleValidate = () => {
    setFocusedIssue(null);
    setValidationOpen(true);
  };

  const handleFocusIssue = (index: number) => {
    const next = focusedIssue === index ? null : index;
    setFocusedIssue(next);
    const keys = validationIssues?.[index]?.cellKeys ?? [];
    if (next === null) return;
    requestAnimationFrame(() => {
      for (const key of keys) {
        const el = document.querySelector<HTMLElement>(`[data-seat-key="${key}"]`);
        if (el) {
          el.scrollIntoView({ block: "center", behavior: "smooth" });
          break;
        }
      }
    });
  };

  const handleEscape = () => {
    if (isPickingCopySource || editor.copiedRow) {
      handleCancelCopyRow();
    } else if (editor.selectedKeys.size > 0) {
      editor.clearSelection();
    } else if (selectionMode) {
      handleToggleSelectionMode();
    } else if (validationOpen) {
      closeValidation();
    }
  };

  useSeatEditorShortcuts({
    enabled: !previewMode,
    seatTypes,
    hasSelection: editor.selectedKeys.size > 0,
    onSelectBrush: setBrush,
    onUndo: editor.undo,
    onRedo: editor.redo,
    onSave: () => {
      if (!updateSeatLayout.isPending) void handleSave();
    },
    onSelectAll: () => {
      setSelectionMode(true);
      editor.selectAll();
    },
    onDisableSelection: editor.disableSelection,
    onEscape: handleEscape,
  });

  return (
    <div className="flex flex-col gap-4">
      <SeatEditorToolbar
        seatTypes={seatTypes}
        brush={brush}
        onBrushChange={setBrush}
        rows={editor.rows}
        cols={editor.cols}
        onResize={editor.resize}
        onSave={handleSave}
        isSaving={updateSeatLayout.isPending}
        isDirty={editor.isDirty}
        canUndo={editor.canUndo}
        canRedo={editor.canRedo}
        onUndo={editor.undo}
        onRedo={editor.redo}
        previewMode={previewMode}
        onTogglePreview={handleTogglePreview}
        selectionMode={selectionMode}
        onToggleSelectionMode={handleToggleSelectionMode}
        isPickingCopySource={isPickingCopySource}
        copySourceRow={editor.copiedRow?.sourceRow ?? null}
        onStartCopyRow={handleStartCopyRow}
        onCancelCopyRow={handleCancelCopyRow}
        onValidate={handleValidate}
        defaultSeatTypeId={defaultSeatTypeId}
        onAutoGenerate={editor.autoGenerate}
      />

      {validationIssues && (
        <SeatEditorValidationPanel
          issues={validationIssues}
          focusedIndex={focusedIssueIndex}
          onFocusIssue={handleFocusIssue}
          onClose={closeValidation}
        />
      )}

      <SeatEditorStats cellMap={editor.cellMap} seatTypes={seatTypes} />

      <SeatEditorGrid
        rows={editor.rows}
        cols={editor.cols}
        cellMap={editor.cellMap}
        seatTypes={seatTypes}
        pendingCoupleKey={editor.pendingCoupleKey}
        onCellClick={handleCellClick}
        activeBrush={brush}
        issueKeys={issueKeys}
        previewMode={previewMode}
        selectionMode={selectionMode}
        selectedKeys={editor.selectedKeys}
        onSelectionChange={editor.setSelectionKeys}
        onRowLabelClick={handleRowLabelClick}
        onColLabelClick={handleColLabelClick}
        copySourceRow={editor.copiedRow?.sourceRow ?? null}
        isPickingCopySource={isPickingCopySource}
      />

      {selectionMode && !previewMode && (
        <SeatEditorSelectionBar
          className="sticky bottom-4 z-20 shadow-lg"
          selectedCount={editor.selectedKeys.size}
          brushLabel={describeBrush(brush, seatTypes).label}
          onApply={handleApplyToSelection}
          onClear={editor.clearSelection}
          onSelectAll={editor.selectAll}
          onDisable={editor.disableSelection}
        />
      )}

      <Button variant="outline" className="w-fit" onClick={() => navigate(ROUTES.admin.rooms)}>
        <ArrowLeft className="size-4" />
        <span>Quay lại danh sách phòng chiếu</span>
      </Button>
    </div>
  );
};

const SeatEditorLoader = ({ room }: { room: AdminRoomDto }) => {
  const roomSeats = useAdminRoomSeats(room.roomId);
  const seatTypes = useAdminSeatTypes();

  if (roomSeats.isLoading || seatTypes.isLoading) {
    return <Skeleton className="h-96 w-full " />;
  }

  if (roomSeats.isError) {
    return (
      <p className="border border-dashed border-destructive/40 py-10 text-center text-sm text-destructive">
        Không tải được sơ đồ ghế hiện tại, vui lòng thử tải lại trang.
      </p>
    );
  }

  return (
    <SeatEditorReady room={room} seats={roomSeats.data ?? []} seatTypes={seatTypes.data?.items ?? []} />
  );
};

const AdminRoomSeatsPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const roomId = Number(id);
  const room = useAdminRoom(roomId);

  return (
    <div>
      <PageHeader
        title={room.data ? `Sơ đồ ghế — ${room.data.roomName}` : "Sơ đồ ghế"}
        description="Chỉnh loại ghế, ghế đôi, lối đi và bật/tắt từng ghế trong phòng chiếu."
      />

      {room.isLoading ? (
        <Skeleton className="h-96 w-full " />
      ) : !room.data ? (
        <div className="flex flex-col items-center gap-3 border border-dashed border-border py-16 text-center">
          <p className="text-sm text-muted-foreground">Không tìm thấy phòng chiếu này.</p>
          <Button onClick={() => navigate(ROUTES.admin.rooms)}>
            <ArrowLeft className="size-4" />
            <span>Về danh sách phòng chiếu</span>
          </Button>
        </div>
      ) : (
        <SeatEditorLoader room={room.data} />
      )}
    </div>
  );
};

export default AdminRoomSeatsPage;
