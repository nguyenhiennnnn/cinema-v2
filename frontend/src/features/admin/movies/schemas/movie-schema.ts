import { z } from "zod";

export const castSchema = z.object({
  actorId: z.coerce.number({ message: "Vui lòng chọn diễn viên." }),
  characterName: z.string().optional().or(z.literal("")),
});

export const movieFormSchema = z.object({
  title: z.string().min(1, "Vui lòng nhập tên phim.").max(200),
  originalTitle: z.string().optional().or(z.literal("")),
  description: z.string().optional().or(z.literal("")),
  directorId: z.coerce.number().optional(),
  durationMinutes: z.coerce.number().min(1, "Thời lượng tối thiểu 1 phút."),
  releaseDate: z.string().optional().or(z.literal("")),
  languageId: z.coerce.number().optional(),
  subtitle: z.string().optional().or(z.literal("")),
  ageRatingId: z.coerce.number().optional(),
  posterUrl: z.string().optional().or(z.literal("")),
  trailerUrl: z.string().optional().or(z.literal("")),
  genreIds: z.array(z.number()).min(1, "Chọn ít nhất một thể loại."),
  cast: z.array(castSchema),
});

export const movieUpdateFormSchema = movieFormSchema.extend({
  status: z.string().min(1, "Vui lòng chọn trạng thái."),
});

export type MovieCreateFormValues = z.infer<typeof movieFormSchema>;
export type MovieUpdateFormValues = z.infer<typeof movieUpdateFormSchema>;
