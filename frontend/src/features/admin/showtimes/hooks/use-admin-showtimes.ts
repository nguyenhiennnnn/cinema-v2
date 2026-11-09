import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import type {
  AdminShowtimeListParams,
  AdminShowtimeCalendarParams,
  UpsertShowtimeRequest,
  BulkCreateShowtimesRequest,
  BulkDeleteShowtimesRequest,
  AutoScheduleRequest,
  ApplyAutoScheduleRequest,
  CloneShowtimesRequest,
} from "../types";
import { showtimesAdminApi } from "../api/showtimes-api";

export function useAdminShowtimes(params: AdminShowtimeListParams) {
  return useQuery({
    queryKey: queryKeys.adminShowtimes.list(params),
    queryFn: () => showtimesAdminApi.getShowtimes(params),
    select: (res) => res.data,
  });
}

export function useAdminShowtimesCalendar(params: AdminShowtimeCalendarParams) {
  return useQuery({
    queryKey: queryKeys.adminShowtimes.calendar(params),
    queryFn: () => showtimesAdminApi.getCalendar(params),
    select: (res) => res.data,
    enabled: Boolean(params.from && params.to),
  });
}

export function useAdminCreateShowtime() {
  return useMutation({
    meta: { invalidates: "showtimeChanged" },
    mutationFn: (body: UpsertShowtimeRequest) =>
      showtimesAdminApi.createShowtime(body),
  });
}

export function useAdminUpdateShowtime() {
  return useMutation({
    meta: { invalidates: "showtimeChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertShowtimeRequest }) =>
      showtimesAdminApi.updateShowtime(id, body),
  });
}

export function useAdminDeleteShowtime() {
  return useMutation({
    meta: { invalidates: "showtimeCancelled" },
    mutationFn: (id: number) => showtimesAdminApi.deleteShowtime(id),
  });
}

export function useAdminBulkCreateShowtimes() {
  return useMutation({
    meta: { invalidates: "showtimeChanged", skipSuccessToast: true },
    mutationFn: (body: BulkCreateShowtimesRequest) =>
      showtimesAdminApi.bulkCreate(body),
  });
}

export function useAdminBulkDeleteShowtimes() {
  return useMutation({
    meta: { invalidates: "showtimeCancelled" },
    mutationFn: (body: BulkDeleteShowtimesRequest) =>
      showtimesAdminApi.bulkDelete(body),
  });
}

export function useAdminPreviewAutoSchedule() {
  return useMutation({
    mutationFn: (body: AutoScheduleRequest) =>
      showtimesAdminApi.previewAutoSchedule(body),
  });
}

export function useAdminApplyAutoSchedule() {
  return useMutation({
    meta: { invalidates: "showtimeChanged", skipSuccessToast: true },
    mutationFn: (body: ApplyAutoScheduleRequest) =>
      showtimesAdminApi.applyAutoSchedule(body),
  });
}

export function useAdminCloneShowtimes() {
  return useMutation({
    meta: { invalidates: "showtimeChanged", skipSuccessToast: true },
    mutationFn: (body: CloneShowtimesRequest) =>
      showtimesAdminApi.cloneShowtimes(body),
  });
}
