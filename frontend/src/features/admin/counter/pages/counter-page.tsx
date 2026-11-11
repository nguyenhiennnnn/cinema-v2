import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";

import { useCounterFlow } from "../hooks/use-counter-flow";
import { CounterShowtimePicker } from "../components/counter-showtime-picker";
import { CounterSeatsStep } from "../components/counter-seats-step";
import { CounterCheckoutStep } from "../components/counter-checkout-step";
import { CounterTicketResult } from "../components/counter-ticket-result";

const AdminCounterPage = () => {
  const flow = useCounterFlow();

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Bán vé tại quầy"
        description="Chọn suất chiếu, chọn ghế và lập đơn bán vé trực tiếp cho khách."
        action={
          flow.step !== "showtime" ? (
            <Button variant="outline" onClick={flow.resetFlow} disabled={flow.isCancelling || flow.isCheckingOut}>
              Bắt đầu lại
            </Button>
          ) : undefined
        }
      />

      {flow.step === "showtime" && (
        <CounterShowtimePicker
          date={flow.selectedDate}
          onDateChange={flow.setSelectedDate}
          onPickShowtime={flow.pickShowtime}
        />
      )}

      {flow.step === "seats" && <CounterSeatsStep flow={flow} />}

      {flow.step === "checkout" && <CounterCheckoutStep flow={flow} />}

      {flow.step === "done" && flow.checkoutResult && (
        <CounterTicketResult
          isLoading={flow.bookingDetail.isLoading}
          booking={flow.bookingDetail.data}
          checkout={flow.checkoutResult}
          onNewSale={flow.resetFlow}
        />
      )}
    </div>
  );
};

export default AdminCounterPage;
