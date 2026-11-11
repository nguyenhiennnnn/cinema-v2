import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { ComboPicker } from "@/components/common/combo-picker/combo-picker";
import { useActiveCombos } from "@/features/client/combos/hooks/use-combos";

import { useCounterFlow } from "../hooks/use-counter-flow";
import { CounterShowtimePicker } from "../components/counter-showtime-picker";
import { CounterSeatPanel } from "../components/counter-seat-panel";
import { CounterCustomerForm } from "../components/counter-customer-form";
import { CounterPriceSummary } from "../components/counter-price-summary";
import { CounterCheckoutPanel } from "../components/counter-checkout-panel";
import { CounterTicketResult } from "../components/counter-ticket-result";

const AdminCounterPage = () => {
  const flow = useCounterFlow();
  const activeCombos = useActiveCombos();

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

      {flow.step === "seats" && (
        <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
          <div className="lg:col-span-2">
            <CounterSeatPanel
              isLoading={flow.showtimeSeats.isLoading}
              data={flow.showtimeSeats.data}
              statusMap={flow.seatStatusMap}
              selectedIds={flow.selectedSeatIds}
              onToggleSeat={flow.toggleSeat}
            />
          </div>

          <div className="flex flex-col gap-4">
            <div className="bg-card p-4">
              <h3 className="mb-3 font-heading text-sm font-semibold text-foreground">Combo</h3>
              <ComboPicker
                combos={activeCombos.data}
                isLoading={activeCombos.isLoading}
                quantities={flow.comboQuantities}
                onChange={flow.setComboQuantity}
              />
            </div>

            <div className="bg-card p-4">
              <h3 className="mb-3 font-heading text-sm font-semibold text-foreground">Thông tin khách</h3>
              <CounterCustomerForm
                customerName={flow.customerName}
                onCustomerNameChange={flow.setCustomerName}
                customerPhone={flow.customerPhone}
                onCustomerPhoneChange={flow.setCustomerPhone}
                userId={flow.userId}
                onUserIdChange={flow.setUserId}
              />
            </div>

            <CounterPriceSummary
              seats={flow.selectedSeatsInfo}
              combos={activeCombos.data}
              comboQuantities={flow.comboQuantities}
            />

            <Button disabled={!flow.canHold || flow.isHolding} onClick={flow.holdSeats}>
              {flow.isHolding ? "Đang giữ ghế..." : "Giữ ghế & tiếp tục"}
            </Button>
          </div>
        </div>
      )}

      {flow.step === "checkout" && flow.holdResult && (
        <div className="mx-auto flex w-full max-w-md flex-col gap-4">
          <CounterPriceSummary
            seats={flow.selectedSeatsInfo}
            combos={activeCombos.data}
            comboQuantities={flow.comboQuantities}
            finalAmount={flow.holdResult.finalAmount}
          />
          <CounterCheckoutPanel
            hold={flow.holdResult}
            onConfirm={flow.confirmCheckout}
            onChangeSeats={flow.backToSeats}
            onCancelOrder={flow.resetFlow}
            isSubmitting={flow.isCheckingOut || flow.isCancelling}
          />
        </div>
      )}

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
