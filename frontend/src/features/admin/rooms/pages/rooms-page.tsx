import { Plus } from "lucide-react";

import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast";
import { DataTable } from "@/components/common/data-table/data-table";
import { useDialogClose, useDialogOpen } from "@/stores/dialog-store";
import { useAlertDialogOpen } from "@/stores/alert-dialog-store";

import { useTablePagination } from "@/hooks/use-table-pagination";

import { useAdminRooms, useAdminUpdateRoom } from "../hooks/use-admin-rooms";
import type { AdminRoomDto } from "../types";
import { createRoomColumns } from "../components/room-columns";
import { RoomCreateForm } from "../components/forms/room-create-form";
import { RoomUpdateForm } from "../components/forms/room-update-form";
import { useAdminListParams } from "@/hooks/use-query-params";
import { useNavigate } from "react-router";

const AdminRoomsPage = () => {
  const [params, setParams] = useAdminListParams();
  const { pagination, onPaginationChange } = useTablePagination(params, setParams);
  const navigate = useNavigate();

  const rooms = useAdminRooms(params);
  const updateRoom = useAdminUpdateRoom();

  const openDialog = useDialogOpen();
  const closeDialog = useDialogClose();
  const openAlertDialog = useAlertDialogOpen();

  const handleCreate = () => {
    openDialog({
      title: "Thêm phòng chiếu mới",
      description: "Nhập tên và loại màn hình cho phòng chiếu.",
      content: <RoomCreateForm onSuccess={closeDialog} />,
    });
  };

  const handleEdit = (room: AdminRoomDto) => {
    openDialog({
      title: "Cập nhật phòng chiếu",
      description: `Chỉnh sửa thông tin phòng "${room.roomName}".`,
      content: <RoomUpdateForm room={room} onSuccess={closeDialog} />,
    });
  };

  const handleToggle = (room: AdminRoomDto) => {
    const willActivate = !room.isActive;
    openAlertDialog({
      title: willActivate
        ? `Kích hoạt lại phòng chiếu "${room.roomName}"?`
        : `Tạm ngưng phòng chiếu "${room.roomName}"?`,
      description: willActivate
        ? "Phòng chiếu sẽ có thể được xếp lịch suất chiếu trở lại."
        : "Phòng chiếu sẽ không thể tạo suất chiếu mới cho đến khi được kích hoạt lại.",
      confirmText: willActivate ? "Kích hoạt" : "Tạm ngưng",
      variant: willActivate ? "default" : "destructive",
      onConfirm: async () => {
        await updateRoom.mutateAsync({
          id: room.roomId,
          body: {
            roomName: room.roomName,
            screenTypeId: null,
            isActive: willActivate,
          },
        });
        toast.add({
          title: willActivate ? "Đã kích hoạt phòng chiếu." : "Đã tạm ngưng phòng chiếu.",
          type: "success",
        });
      },
    });
  };

  const handleViewDetail = (room: AdminRoomDto) => {
    navigate(`/admin/rooms/${room.roomId}/seats`, { state: { room } });
  };

  const columns = createRoomColumns({
    onEdit: handleEdit,
    onToggle: handleToggle,
    onViewDetail: handleViewDetail,
  });

  return (
    <div>
      <PageHeader
        title="Phòng chiếu"
        description="Quản lý danh sách phòng chiếu và loại màn hình."
        action={
          <Button onClick={handleCreate}>
            <Plus className="size-4" />
            <span>Thêm phòng chiếu</span>
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={rooms.data?.items ?? []}
        isLoading={rooms.isLoading}
        totalItems={rooms.data?.totalItems ?? 0}
        pagination={pagination}
        onPaginationChange={onPaginationChange}
        emptyMessage="Chưa có phòng chiếu nào."
      />
    </div>
  );
};

export default AdminRoomsPage;