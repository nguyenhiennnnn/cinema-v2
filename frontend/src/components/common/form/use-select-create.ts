import { useEffect, useRef, useState } from "react";
import {
  describeCreateError,
  isSameName,
  normalizeName,
  validateNewName,
} from "./select-name-utils";

export interface SelectCreateProps<T> {
  onCreate?: (name: string) => Promise<T>;
  canCreate?: boolean;
  createMaxLength?: number;
}

interface UseSelectCreateOptions<T> extends SelectCreateProps<T> {
  options: T[];
  getOptionLabel: (option: T) => string;
  searchValue: string;
  onCreated: (option: T) => void;
}

export function useSelectCreate<T>({
  options,
  getOptionLabel,
  searchValue,
  onCreate,
  canCreate = true,
  createMaxLength,
  onCreated,
}: UseSelectCreateOptions<T>) {
  const [isCreating, setIsCreating] = useState(false);
  const [createError, setCreateError] = useState<string | null>(null);
  const [created, setCreated] = useState<T[]>([]);
  const lockRef = useRef(false);
  const mountedRef = useRef(true);

  useEffect(() => {
    mountedRef.current = true;
    return () => {
      mountedRef.current = false;
    };
  }, []);

  useEffect(() => {
    setCreateError(null);
  }, [searchValue]);

  const allOptions = [
    ...options,
    ...created.filter(
      (c) => !options.some((o) => getOptionLabel(o) === getOptionLabel(c)),
    ),
  ];

  const term = normalizeName(searchValue);
  const exactMatch = term
    ? allOptions.find((o) => isSameName(getOptionLabel(o), term))
    : undefined;
  const termError = term ? validateNewName(term, createMaxLength) : null;
  const showCreate = Boolean(
    onCreate && canCreate && term && !exactMatch && !termError,
  );

  const create = async () => {
    if (!onCreate || lockRef.current || !term || termError) return;
    if (exactMatch) {
      onCreated(exactMatch);
      return;
    }

    lockRef.current = true;
    setIsCreating(true);
    setCreateError(null);
    try {
      const option = await onCreate(term);
      if (!mountedRef.current) return;
      setCreated((prev) => [...prev, option]);
      onCreated(option);
    } catch (error) {
      if (mountedRef.current) setCreateError(describeCreateError(error));
    } finally {
      lockRef.current = false;
      if (mountedRef.current) setIsCreating(false);
    }
  };

  return {
    allOptions,
    term,
    showCreate,
    termError: showCreate || !term ? null : termError,
    isCreating,
    createError,
    create,
  };
}
