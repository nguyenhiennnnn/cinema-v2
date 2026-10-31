export interface Inset {
  top: number;
  right: number;
  bottom: number;
  left: number;
}

export function measureInset(hero: DOMRect, el: DOMRect): Inset {
  return {
    top: el.top - hero.top,
    left: el.left - hero.left,
    right: hero.right - el.right,
    bottom: hero.bottom - el.bottom,
  };
}

export function buildClipFrames(width: number, height: number, o: Inset, cut: number) {
  const x1 = o.left;
  const y1 = o.top;
  const x2 = width - o.right;
  const y2 = height - o.bottom;
  const from = `polygon(${x1 + cut}px ${y1}px, ${x2}px ${y1}px, ${x2}px ${y2 - cut}px, ${x2 - cut}px ${y2}px, ${x1}px ${y2}px, ${x1}px ${y1 + cut}px)`;
  const to = `polygon(0px 0px, ${width}px 0px, ${width}px ${height}px, ${width}px ${height}px, 0px ${height}px, 0px 0px)`;
  return [{ clipPath: from }, { clipPath: to }];
}
