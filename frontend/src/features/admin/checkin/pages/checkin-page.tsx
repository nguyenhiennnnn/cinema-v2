import { useCallback, useRef, useState } from "react";
import { Ticket } from "lucide-react";
import { PageHeader } from "@/components/common/page-header";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import type { CheckinResponse } from "../../bookings/types";
import { useCheckin } from "../../bookings/hooks/use-admin-booking";
import { useCheckinSound } from "../hooks/use-checkin-sound";
import { CheckinCodeInput } from "../components/checkin-code-input";
import { CheckinCameraPanel } from "../components/checkin-camera-panel";
import { CheckinResultCard } from "../components/checkin-result-card";


const AdminCheckinPage = () => {
  const [code, setCode] = useState("");
  const [cameraActive, setCameraActive] = useState(false);
  const [status, setStatus] = useState<"idle" | "success" | "error">("idle");
  const [result, setResult] = useState<CheckinResponse | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const checkinMutation = useCheckin();
  const { playSuccess, playError } = useCheckinSound();

  const handleScan = useCallback(
    (qrCode: string) => {
      const trimmed = qrCode.trim();
      if (!trimmed || checkinMutation.isPending) return;

      checkinMutation.mutate(
        { qrCode: trimmed },
        {
          onSuccess: (res) => {
            const data = res.data;
            setStatus("success");
            setResult(data);
            setErrorMessage(null);
            playSuccess();
          },
          onError: (err: unknown) => {
            setStatus("error");
            setResult(null);
            const errorObj = err as { response?: { data?: { message?: string } }; message?: string };
            setErrorMessage(
              errorObj.response?.data?.message ||
                errorObj.message ||
                "Mã vé không hợp lệ hoặc đã được sử dụng",
            );
            playError();
          },
        },
      );

      setCode("");
      inputRef.current?.focus();
    },
    [checkinMutation, playError, playSuccess],
  );

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Soát vé & Check-in"
        description="Quét mã QR hoặc nhập mã vé để xác nhận khách vào rạp."
      />

      <div className="grid gap-6 lg:grid-cols-2">
        <Card className="flex flex-col gap-4 p-5 lg:col-span-1">
          <CheckinCodeInput
            ref={inputRef}
            value={code}
            onChange={setCode}
            onSubmit={() => handleScan(code)}
            disabled={checkinMutation.isPending}
          />

          <Button type="button" onClick={() => handleScan(code)} disabled={!code.trim() || checkinMutation.isPending}>
            <Ticket className="size-4" />
            <span>Soát vé ngay</span>
          </Button>

          <CheckinCameraPanel
            active={cameraActive}
            onToggle={() => setCameraActive((prev) => !prev)}
            onDetect={handleScan}
          />
        </Card>

        <Card className="p-5 lg:col-span-1">
          <CheckinResultCard status={status} result={result} errorMessage={errorMessage} />
        </Card>
      </div>
    </div>
  );
};

export default AdminCheckinPage;