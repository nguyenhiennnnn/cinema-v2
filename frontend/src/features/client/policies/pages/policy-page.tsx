import { Link, Navigate, useParams } from "react-router";
import { Seo } from "@/components/common/seo";
import { cn } from "@/lib/utils";
import { useCinemaSettings } from "@/features/client/settings/hooks/use-settings";
import { getPolicy, POLICIES } from "../policy-content";

const PolicyPage = () => {
  const { slug } = useParams();
  const policy = getPolicy(slug);
  const { data: settings } = useCinemaSettings();

  if (!policy) return <Navigate to="/404" replace />;

  const sections = policy.sections({
    cancelDeadlineHours: settings?.cancelDeadlineHours,
    cancelRefundPercent: settings?.cancelRefundPercent,
    stopSellingMinutesBefore: settings?.stopSellingMinutesBefore,
  });

  return (
    <div className="mx-auto grid max-w-5xl gap-8 px-4 py-10 md:grid-cols-[14rem_1fr] md:py-14">
      <Seo
        title={policy.title}
        description={policy.description}
        canonical={`/policies/${policy.slug}`}
      />

      <aside className="md:sticky md:top-24 md:self-start">
        <p className="mb-3 text-[13px] font-medium text-foreground">Chính sách</p>
        <nav className="flex flex-row flex-wrap gap-1 md:flex-col">
          {POLICIES.map((item) => (
            <Link
              key={item.slug}
              to={`/policies/${item.slug}`}
              aria-current={item.slug === policy.slug ? "page" : undefined}
              className={cn(
                "border border-transparent px-3 py-2 text-sm text-muted-foreground transition-colors hover:text-foreground",
                item.slug === policy.slug &&
                  "border-border bg-card font-medium text-foreground",
              )}
            >
              {item.title}
            </Link>
          ))}
        </nav>
      </aside>

      <article className="min-w-0 border border-border/60 bg-card p-6 md:p-10">
        <h1 className="font-heading text-2xl font-bold tracking-tight text-foreground md:text-3xl">
          {policy.title}
        </h1>
        <p className="mt-1 text-xs text-muted-foreground">
          Cập nhật lần cuối: {policy.updatedAt}
        </p>
        <div className="perf-h my-6" />

        <div className="flex flex-col gap-6">
          {sections.map((section) => (
            <section key={section.heading}>
              <h2 className="mb-2 font-heading text-lg font-semibold tracking-tight text-foreground">
                {section.heading}
              </h2>
              {section.list ? (
                <ul className="list-disc space-y-1.5 pl-5 text-sm leading-relaxed text-muted-foreground">
                  {section.body.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                </ul>
              ) : (
                section.body.map((line) => (
                  <p
                    key={line}
                    className="text-sm leading-relaxed text-muted-foreground"
                  >
                    {line}
                  </p>
                ))
              )}
            </section>
          ))}
        </div>
      </article>
    </div>
  );
};

export default PolicyPage;
