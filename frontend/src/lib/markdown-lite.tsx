import { Fragment, type ReactNode } from "react";

function parseInline(text: string, keyPrefix: string): ReactNode[] {
  const parts = text.split(/(\*\*[^*]+\*\*)/g).filter(Boolean);
  return parts.map((part, i) =>
    part.startsWith("**") && part.endsWith("**") ? (
      <strong key={`${keyPrefix}-b-${i}`}>{part.slice(2, -2)}</strong>
    ) : (
      <Fragment key={`${keyPrefix}-t-${i}`}>{part}</Fragment>
    ),
  );
}

export function renderMarkdownLite(content: string): ReactNode {
  const blocks = content.trim().split(/\n{2,}/);

  return (
    <div className="flex flex-col gap-2">
      {blocks.map((block, blockIndex) => {
        const lines = block.split("\n").filter((l) => l.trim().length > 0);
        const isList = lines.length > 0 && lines.every((l) => /^\s*[-•]\s+/.test(l));

        if (isList) {
          return (
            <ul key={blockIndex} className="ml-4 list-disc space-y-1">
              {lines.map((line, i) => (
                <li key={i}>{parseInline(line.replace(/^\s*[-•]\s+/, ""), `${blockIndex}-${i}`)}</li>
              ))}
            </ul>
          );
        }

        return (
          <p key={blockIndex}>
            {lines.map((line, i) => (
              <Fragment key={i}>
                {i > 0 && <br />}
                {parseInline(line, `${blockIndex}-${i}`)}
              </Fragment>
            ))}
          </p>
        );
      })}
    </div>
  );
}