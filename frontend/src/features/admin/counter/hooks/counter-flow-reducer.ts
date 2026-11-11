export type CounterStep = "showtime" | "seats" | "checkout" | "done";

export type CounterFlowAction =
  | { type: "SET_STEP"; step: CounterStep }
  | { type: "SHOWTIME_PICKED" }
  | { type: "HOLD_SUCCEEDED" }
  | { type: "CHECKOUT_SUCCEEDED" }
  | { type: "BACK_TO_SEATS" }
  | { type: "RESET" };

export const initialCounterStep: CounterStep = "showtime";

export function counterFlowReducer(
  state: CounterStep,
  action: CounterFlowAction,
): CounterStep {
  switch (action.type) {
    case "SET_STEP":
      return action.step;
    case "SHOWTIME_PICKED":
    case "BACK_TO_SEATS":
      return "seats";
    case "HOLD_SUCCEEDED":
      return "checkout";
    case "CHECKOUT_SUCCEEDED":
      return "done";
    case "RESET":
      return "showtime";
    default:
      return state;
  }
}
