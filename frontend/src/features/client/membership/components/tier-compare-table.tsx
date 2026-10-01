import { Check } from "lucide-react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { COMPARE_ROWS } from "../constants";
import type { MembershipTierDto } from "@/features/admin/catalog/types";

interface TierCompareTableProps {
  tiers: MembershipTierDto[];
}

export const TierCompareTable = ({ tiers }: TierCompareTableProps) => {
  return (
    <div className="overflow-x-auto rounded-xl border border-border/60">
      <Table>
        <TableHeader>
          <TableRow className="bg-muted/40 hover:bg-muted/40">
            <TableHead className="w-56 text-foreground">Quyền lợi</TableHead>
            {tiers.map((tier) => (
              <TableHead key={tier.tierId} className="text-center text-foreground">
                {tier.tierName}
              </TableHead>
            ))}
          </TableRow>
        </TableHeader>
        <TableBody>
          {COMPARE_ROWS.map((row) => (
            <TableRow key={row.label}>
              <TableCell className="flex items-center gap-2 text-sm font-medium text-foreground">
                <row.icon className="size-3.5 text-muted-foreground" />
                {row.label}
              </TableCell>
              {tiers.map((tier, index) => (
                <TableCell key={tier.tierId} className="text-center">
                  {index >= row.unlockAt ? (
                    <Check className="mx-auto size-4 text-primary" />
                  ) : (
                    <span className="text-muted-foreground/40">—</span>
                  )}
                </TableCell>
              ))}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}