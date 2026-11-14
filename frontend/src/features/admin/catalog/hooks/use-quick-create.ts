import { useMutation } from "@tanstack/react-query";
import type { InvalidationName } from "@/api/query-invalidation";
import {
  LOOKUP_PAGE_SIZE,
  QUICK_CREATE_AGE_RATING_MAX_LENGTH,
  QUICK_CREATE_NAME_MAX_LENGTH,
  QUICK_CREATE_PERSON_NAME_MAX_LENGTH,
} from "@/config/default-constants";
import { isSameName } from "@/components/common/form/select-name-utils";
import { extractApiError } from "@/lib/error";
import { useAuthStore } from "@/stores/auth-store";
import { Role } from "@/types/enum";
import {
  actorsAdminApi,
  ageRatingsAdminApi,
  directorsAdminApi,
  genresAdminApi,
  languagesAdminApi,
  positionsAdminApi,
  screenTypesAdminApi,
} from "../api/catalog-api";
import type {
  ActorDto,
  AgeRatingDto,
  DirectorDto,
  GenreDto,
  LanguageDto,
  PositionDto,
  ScreenTypeDto,
} from "@/features/admin/catalog/types";

const QUICK_CREATE_ROLE = Role.Admin;

interface QuickCreateConfig<TDto> {
  invalidates: InvalidationName;
  maxLength: number;
  create: (name: string) => Promise<TDto>;
  findExisting?: (name: string) => Promise<TDto | undefined>;
}

function useQuickCreate<TDto>(config: QuickCreateConfig<TDto>) {
  const canCreate = useAuthStore((s) => s.user?.role) === QUICK_CREATE_ROLE;

  const { mutateAsync } = useMutation({
    meta: {
      invalidates: config.invalidates,
      skipErrorToast: true,
      skipSuccessToast: true,
    },
    mutationFn: async (name: string) => {
      try {
        return await config.create(name);
      } catch (error) {
        if (extractApiError(error).statusCode === 409 && config.findExisting) {
          const existing = await config.findExisting(name);
          if (existing) return existing;
        }
        throw error;
      }
    },
  });

  return {
    canCreate,
    onCreate: (name: string) => mutateAsync(name),
    createMaxLength: config.maxLength,
  };
}

const genreConfig: QuickCreateConfig<GenreDto> = {
  invalidates: "genreCreated",
  maxLength: QUICK_CREATE_NAME_MAX_LENGTH,
  create: async (name) => (await genresAdminApi.create({ name })).data,
  findExisting: async (name) =>
    (
      await genresAdminApi.getAll({ pageSize: LOOKUP_PAGE_SIZE })
    ).data.items.find((g) => isSameName(g.name, name)),
};

const languageConfig: QuickCreateConfig<LanguageDto> = {
  invalidates: "languageCreated",
  maxLength: QUICK_CREATE_NAME_MAX_LENGTH,
  create: async (name) => (await languagesAdminApi.create({ name })).data,
  findExisting: async (name) =>
    (
      await languagesAdminApi.getAll({ pageSize: LOOKUP_PAGE_SIZE })
    ).data.items.find((l) => isSameName(l.name, name)),
};

const ageRatingConfig: QuickCreateConfig<AgeRatingDto> = {
  invalidates: "ageRatingCreated",
  maxLength: QUICK_CREATE_AGE_RATING_MAX_LENGTH,
  create: async (code) =>
    (await ageRatingsAdminApi.create({ code, description: null })).data,
  findExisting: async (code) =>
    (
      await ageRatingsAdminApi.getAll({ pageSize: LOOKUP_PAGE_SIZE })
    ).data.items.find((a) => isSameName(a.code, code)),
};

const positionConfig: QuickCreateConfig<PositionDto> = {
  invalidates: "positionCreated",
  maxLength: QUICK_CREATE_NAME_MAX_LENGTH,
  create: async (name) => (await positionsAdminApi.create({ name })).data,
  findExisting: async (name) =>
    (
      await positionsAdminApi.getAll({ pageSize: LOOKUP_PAGE_SIZE })
    ).data.items.find((p) => isSameName(p.name, name)),
};

const screenTypeConfig: QuickCreateConfig<ScreenTypeDto> = {
  invalidates: "screenTypeCreated",
  maxLength: QUICK_CREATE_NAME_MAX_LENGTH,
  create: async (name) => (await screenTypesAdminApi.create({ name })).data,
  findExisting: async (name) =>
    (
      await screenTypesAdminApi.getAll({ pageSize: LOOKUP_PAGE_SIZE })
    ).data.items.find((s) => isSameName(s.name, name)),
};

const directorConfig: QuickCreateConfig<DirectorDto> = {
  invalidates: "directorCreated",
  maxLength: QUICK_CREATE_PERSON_NAME_MAX_LENGTH,
  create: async (fullName) =>
    (await directorsAdminApi.quickCreate({ fullName })).data,
};

const actorConfig: QuickCreateConfig<ActorDto> = {
  invalidates: "actorCreated",
  maxLength: QUICK_CREATE_PERSON_NAME_MAX_LENGTH,
  create: async (fullName) =>
    (await actorsAdminApi.quickCreate({ fullName })).data,
};

export const useGenreQuickCreate = () => useQuickCreate(genreConfig);
export const useLanguageQuickCreate = () => useQuickCreate(languageConfig);
export const useAgeRatingQuickCreate = () => useQuickCreate(ageRatingConfig);
export const usePositionQuickCreate = () => useQuickCreate(positionConfig);
export const useScreenTypeQuickCreate = () => useQuickCreate(screenTypeConfig);
export const useDirectorQuickCreate = () => useQuickCreate(directorConfig);
export const useActorQuickCreate = () => useQuickCreate(actorConfig);
