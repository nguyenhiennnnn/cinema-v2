import { Fragment } from "react";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { getFaqItems } from "../constants";

export function MembershipFaq() {
  return (
    <Accordion className="w-full">
      {getFaqItems().map((item, index) => (
        <Fragment key={item.question}>
          <AccordionItem value={`faq-${index}`}>
            <AccordionTrigger className="text-left text-sm font-medium text-foreground md:text-base">
              {item.question}
            </AccordionTrigger>
            <AccordionContent className="text-sm leading-relaxed text-muted-foreground">
              {item.answer}
            </AccordionContent>
          </AccordionItem>
        </Fragment>
      ))}
    </Accordion>
  );
}