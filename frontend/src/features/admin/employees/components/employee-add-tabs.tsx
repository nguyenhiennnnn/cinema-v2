import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";

import { EmployeeCreateForm } from "./forms/employee-create-form";
import { EmployeeInviteForm } from "./forms/employee-invite-form";

interface EmployeeAddTabsProps {
  onSuccess: () => void;
}

export const EmployeeAddTabs = ({ onSuccess }: EmployeeAddTabsProps) => (
  <Tabs defaultValue="invite">
    <TabsList>
      <TabsTrigger value="invite">Mời qua email</TabsTrigger>
      <TabsTrigger value="existing">Từ tài khoản có sẵn</TabsTrigger>
    </TabsList>

    <TabsContent value="invite">
      <EmployeeInviteForm onSuccess={onSuccess} />
    </TabsContent>

    <TabsContent value="existing">
      <EmployeeCreateForm onSuccess={onSuccess} />
    </TabsContent>
  </Tabs>
);
