import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";

import { EmployeeCreateDirectForm } from "./forms/employee-create-direct-form";
import { EmployeeCreateForm } from "./forms/employee-create-form";
import { EmployeeInviteForm } from "./forms/employee-invite-form";

interface EmployeeAddTabsProps {
  onSuccess: () => void;
}

export const EmployeeAddTabs = ({ onSuccess }: EmployeeAddTabsProps) => (
  <Tabs defaultValue="direct">
    <TabsList>
      <TabsTrigger value="direct">Tạo trực tiếp</TabsTrigger>
      <TabsTrigger value="invite">Mời qua email</TabsTrigger>
      <TabsTrigger value="existing">Từ tài khoản có sẵn</TabsTrigger>
    </TabsList>

    <TabsContent value="direct">
      <EmployeeCreateDirectForm onSuccess={onSuccess} />
    </TabsContent>

    <TabsContent value="invite">
      <EmployeeInviteForm onSuccess={onSuccess} />
    </TabsContent>

    <TabsContent value="existing">
      <EmployeeCreateForm onSuccess={onSuccess} />
    </TabsContent>
  </Tabs>
);
