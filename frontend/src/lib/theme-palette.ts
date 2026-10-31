export const readThemePalette = (tokens: string[]) => {
  const style = getComputedStyle(document.documentElement);
  return tokens.map((token) => style.getPropertyValue(token).trim()).filter(Boolean);
};
