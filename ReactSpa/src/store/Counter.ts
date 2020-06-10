const INCREMENT_COUNT = 'INCREMENT_COUNT';
const DECREMENT_COUNT = 'DECREMENT_COUNT';

// STATE - This defines the type of data maintained in the Redux store.
export interface CounterState {
  count: number;
  isLoading: boolean;
}

// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
interface IncrementCountAction {
  type: typeof INCREMENT_COUNT;
}

interface DecrementCountAction {
  type: typeof DECREMENT_COUNT;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
export type CounterActionTypes = IncrementCountAction | DecrementCountAction;

const initialState: CounterState = {
  count: 0,
  isLoading: false,
};

// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).
export const actionCreators = {
  increment: (): CounterActionTypes => ({
    type: INCREMENT_COUNT,
  }),
  decrement: (): CounterActionTypes => ({
    type: DECREMENT_COUNT,
  }),
};

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
export const reducer = (state = initialState, action: CounterActionTypes): CounterState => {
  switch (action.type) {
    case INCREMENT_COUNT:
      return {...state, count: state.count + 1};
    case DECREMENT_COUNT:
      return {...state, count: state.count - 1};
    default:
      return state;
  }
};
